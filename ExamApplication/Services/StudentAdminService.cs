using ExamApplication.DTO.Student;
using ExamApplication.Helper;
using ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Services;
using ExamDomain.Entities;
using ExamDomain.Enum;

namespace ExamInfrastucture.Services
{
    public class StudentAdminService : IStudentAdminService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentAdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<StudentListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var students = await _unitOfWork.Students.GetAllWithDetailsAsync(cancellationToken);

            return students
                .OrderBy(x => x.FullName)
                .Select(MapToListItemDto)
                .ToList();
        }

        public async Task<StudentDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var student = await _unitOfWork.Students.GetByIdWithDetailsAsync(id, cancellationToken);

            if (student == null)
                throw new KeyNotFoundException("Şagird tapılmadı.");

            return MapToDetailDto(student);
        }

        public async Task<StudentDetailDto> CreateAsync(CreateStudentDto request, CancellationToken cancellationToken = default)
        {
            await ValidateCreateAsync(request, cancellationToken);

            var student = new Student
            {
                UserId = request.UserId,
                FullName = request.FullName.Trim(),
                DateOfBirth = request.DateOfBirth,
                StudentNumber = request.StudentNumber.Trim(),
                Status = request.Status.HasValue
                    ? (StudentStatus)request.Status.Value
                    : StudentStatus.Active,
                Notes = request.Notes?.Trim()
            };

            await _unitOfWork.Students.AddAsync(student, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (request.ClassRoomId.HasValue)
            {
                await SyncStudentClassInternalAsync(student.Id, request.ClassRoomId.Value, cancellationToken);
            }

            return await GetByIdAsync(student.Id, cancellationToken);
        }

        public async Task<StudentDetailDto> UpdateAsync(UpdateStudentDto request, CancellationToken cancellationToken = default)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(request.Id, cancellationToken);

            if (student == null)
                throw new KeyNotFoundException("Şagird tapılmadı.");

            await ValidateUpdateAsync(request, cancellationToken);

            student.FullName = request.FullName.Trim();
            student.DateOfBirth = request.DateOfBirth;
            student.StudentNumber = request.StudentNumber.Trim();

            if (request.Status.HasValue)
            {
                student.Status = (StudentStatus)request.Status.Value;
            }

            student.Notes = request.Notes?.Trim();

            // YENI: Student statusuna gore User.IsActive sinxronlasdirilir
            // Aktiv -> true
            // Passiv / Məzun -> false
            if (request.Status.HasValue)
            {
                var linkedUser = await _unitOfWork.Users.GetByIdAsync(student.UserId, cancellationToken);

                if (linkedUser != null)
                {
                    linkedUser.IsActive = student.Status == StudentStatus.Active;
                    _unitOfWork.Users.Update(linkedUser);
                }
            }

            _unitOfWork.Students.Update(student);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (request.ClassRoomId.HasValue)
            {
                await SyncStudentClassInternalAsync(student.Id, request.ClassRoomId.Value, cancellationToken);
            }
            else
            {
                await ClearActiveStudentClassesInternalAsync(student.Id, cancellationToken);
            }

            return await GetByIdAsync(student.Id, cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var student = await _unitOfWork.Students.GetByIdWithDetailsAsync(id, cancellationToken);

            if (student == null)
                throw new KeyNotFoundException("Şagird tapılmadı.");

            // Aktiv sinif əlaqələrini bağlayırıq
            var activeRelations = await _unitOfWork.StudentClasses.GetActiveByStudentIdAsync(id, cancellationToken);
            foreach (var relation in activeRelations)
            {
                relation.IsActive = false;
                relation.LeftAt = DateTime.UtcNow;
                _unitOfWork.StudentClasses.Update(relation);
            }

            // Şagirdə bağlı məlumatlar varsa hard delete etmirik, deaktiv edirik
            var hasBlockingRelations = await HasBlockingRelationsAsync(student, cancellationToken);

            student.Status = StudentStatus.Passive;
            _unitOfWork.Students.Update(student);

            if (student.User != null)
            {
                student.User.IsActive = false;
                _unitOfWork.Users.Update(student.User);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // İstəsən burada mesajı controller tərəfində "deaktiv edildi" kimi göstərəcəksən
        }

        public async Task<List<StudentOptionDto>> SearchAsync(string? search, CancellationToken cancellationToken = default)
        {
            var students = string.IsNullOrWhiteSpace(search)
                ? await _unitOfWork.Students.GetAllWithDetailsAsync(cancellationToken)
                : await _unitOfWork.Students.SearchByFullNameAsync(search.Trim(), cancellationToken);

            return students
                .OrderBy(x => x.FullName)
                .Select(MapToOptionDto)
                .ToList();
        }

        public async Task<List<StudentOptionDto>> GetOptionsAsync(CancellationToken cancellationToken = default)
        {
            var students = await _unitOfWork.Students.GetAllWithDetailsAsync(cancellationToken);

            return students
                .OrderBy(x => x.FullName)
                .Select(MapToOptionDto)
                .ToList();
        }

        private async Task ValidateCreateAsync(CreateStudentDto request, CancellationToken cancellationToken)
        {
            if (request.UserId <= 0)
                throw new ArgumentException("UserId düzgün deyil.");

            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new ArgumentException("FullName boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(request.StudentNumber))
                throw new ArgumentException("StudentNumber boş ola bilməz.");

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                throw new InvalidOperationException("Bağlanacaq user tapılmadı.");

            if (user.Student != null)
                throw new InvalidOperationException("Bu user artıq student profilinə bağlıdır.");

            var studentNumberExists = await _unitOfWork.Students.ExistsByStudentNumberAsync(request.StudentNumber.Trim(), cancellationToken);
            if (studentNumberExists)
                throw new InvalidOperationException("Bu student nömrəsi artıq mövcuddur.");

            if (request.ClassRoomId.HasValue)
            {
                var classRoom = await _unitOfWork.ClassRooms.GetByIdAsync(request.ClassRoomId.Value, cancellationToken);
                if (classRoom == null)
                    throw new InvalidOperationException("Seçilmiş sinif tapılmadı.");
            }
        }

        private async Task ValidateUpdateAsync(UpdateStudentDto request, CancellationToken cancellationToken)
        {
            if (request.Id <= 0)
                throw new ArgumentException("Student Id düzgün deyil.");

            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new ArgumentException("FullName boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(request.StudentNumber))
                throw new ArgumentException("StudentNumber boş ola bilməz.");

            var studentNumberExists = await _unitOfWork.Students.ExistsByStudentNumberAsync(
                request.StudentNumber.Trim(),
                request.Id,
                cancellationToken);

            if (studentNumberExists)
                throw new InvalidOperationException("Bu student nömrəsi artıq mövcuddur.");

            if (request.ClassRoomId.HasValue)
            {
                var classRoom = await _unitOfWork.ClassRooms.GetByIdAsync(request.ClassRoomId.Value, cancellationToken);
                if (classRoom == null)
                    throw new InvalidOperationException("Seçilmiş sinif tapılmadı.");
            }
        }

        private async Task SyncStudentClassInternalAsync(int studentId, int classRoomId, CancellationToken cancellationToken)
        {
            var activeRelations = await _unitOfWork.StudentClasses.GetActiveByStudentIdAsync(studentId, cancellationToken);

            var alreadyInSameClass = activeRelations.Any(x => x.ClassRoomId == classRoomId && x.IsActive);
            if (alreadyInSameClass)
                return;

            foreach (var relation in activeRelations)
            {
                relation.IsActive = false;
                relation.LeftAt = DateTime.UtcNow;
                _unitOfWork.StudentClasses.Update(relation);
            }

            await _unitOfWork.StudentClasses.AddAsync(new StudentClass
            {
                StudentId = studentId,
                ClassRoomId = classRoomId,
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task ClearActiveStudentClassesInternalAsync(int studentId, CancellationToken cancellationToken)
        {
            var activeRelations = await _unitOfWork.StudentClasses.GetActiveByStudentIdAsync(studentId, cancellationToken);

            foreach (var relation in activeRelations)
            {
                relation.IsActive = false;
                relation.LeftAt = DateTime.UtcNow;
                _unitOfWork.StudentClasses.Update(relation);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private StudentListItemDto MapToListItemDto(Student student)
        {
            var activeClass = student.StudentClasses?
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.JoinedAt)
                .FirstOrDefault();

            var completedExams = student.StudentExams?
                .Where(x => x.IsCompleted)
                .ToList() ?? new List<StudentExam>();

            var attendanceRecords = student.AttendanceRecords?.ToList() ?? new List<AttendanceRecord>();

            var averageScore = completedExams.Count > 0
                ? Math.Round(completedExams.Average(x => x.Score), 2)
                : 0;

            return new StudentListItemDto
            {
                Id = student.Id,
                UserId = student.UserId,
                FullName = student.FullName,
                Email = student.User?.Email ?? string.Empty,
                StudentNumber = student.StudentNumber,
                ClassName = activeClass?.ClassRoom?.Name,
                AverageScore = averageScore,
                ExamsCount = completedExams.Count,
                AttendanceRate = CalculateAttendanceRate(attendanceRecords),
                Status = MapStudentStatus(student.Status),
                PhotoUrl = student.User?.PhotoUrl
            };
        }
        public async Task<StudentAttendanceSummaryDto> UpdateAttendanceAsync(
    int studentId,
    UpdateStudentAttendanceRecordDto request,
    CancellationToken cancellationToken = default)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId, cancellationToken);
            if (student == null)
                throw new KeyNotFoundException("Şagird tapılmadı.");

            var attendanceRecord = await _unitOfWork.AttendanceRecords
                .GetBySessionAndStudentAsync(request.AttendanceSessionId, studentId, cancellationToken);

            if (attendanceRecord == null)
                throw new KeyNotFoundException("Davamiyyət qeydi tapılmadı.");

            if (request.Status < 1 || request.Status > 3)
                throw new ArgumentException("Davamiyyət statusu yanlışdır.");

            attendanceRecord.Status = (ExamDomain.Enum.AttendanceStatus)request.Status;
            attendanceRecord.Notes = string.IsNullOrWhiteSpace(request.Note)
                ? null
                : request.Note.Trim();

            _unitOfWork.AttendanceRecords.Update(attendanceRecord);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updatedRecord = await _unitOfWork.AttendanceRecords
                .GetBySessionAndStudentAsync(request.AttendanceSessionId, studentId, cancellationToken);

            if (updatedRecord == null)
                throw new KeyNotFoundException("Yenilənmiş davamiyyət qeydi tapılmadı.");

            return new StudentAttendanceSummaryDto
            {
                AttendanceSessionId = updatedRecord.AttendanceSessionId,
                SessionDate = updatedRecord.AttendanceSession != null
    ? AzerbaijanTimeHelper.ToBakuTime(updatedRecord.AttendanceSession.SessionDate)
    : DateTime.MinValue,
                SubjectName = updatedRecord.AttendanceSession?.Subject?.Name ?? string.Empty,
                TeacherName = updatedRecord.AttendanceSession?.Teacher?.FullName ?? string.Empty,
                Status = MapAttendanceStatus(updatedRecord.Status),
                StartTime = updatedRecord.AttendanceSession?.StartTime,
                EndTime = updatedRecord.AttendanceSession?.EndTime,
                Note = updatedRecord.Notes
            };
        }
        private StudentDetailDto MapToDetailDto(Student student)
        {
            var activeClass = student.StudentClasses?
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.JoinedAt)
                .FirstOrDefault();

            var completedExams = student.StudentExams?
                .Where(x => x.IsCompleted)
                .ToList() ?? new List<StudentExam>();

            var attendanceRecords = student.AttendanceRecords?.ToList() ?? new List<AttendanceRecord>();

            // YENI
            var tasks = student.Tasks?.ToList() ?? new List<ExamDomain.Entities.StudentTask>();

            var averageScore = completedExams.Count > 0
                ? Math.Round(completedExams.Average(x => x.Score), 2)
                : 0;

            // YENI
            var completedTasksCount = tasks.Count(x =>
                x.Status == ExamDomain.Enum.StudentTaskStatus.Submitted ||
                x.Status == ExamDomain.Enum.StudentTaskStatus.Reviewed);

            // YENI
            var absentCount = attendanceRecords.Count(x => x.Status == AttendanceStatus.Absent);

            // YENI
            var lateCount = attendanceRecords.Count(x => x.Status == AttendanceStatus.Late);

            return new StudentDetailDto
            {
                Id = student.Id,
                UserId = student.UserId,
                FullName = student.FullName,

                // YENI
                FirstName = student.User?.FirstName ?? string.Empty,

                // YENI
                LastName = student.User?.LastName ?? string.Empty,

                Email = student.User?.Email ?? string.Empty,

                // YENI
                PhoneNumber = student.User?.PhoneNumber,

                // YENI
                ParentName = student.User?.FatherName ?? string.Empty,

                // YENI
                ParentPhone = student.User?.ParentPhone,

                // YENI
                Address = student.User?.Address,

                // YENI
                Gender = MapGender(student.User?.Gender),

                StudentNumber = student.StudentNumber,
                DateOfBirth = student.DateOfBirth,
                ClassName = activeClass?.ClassRoom?.Name,
                Status = MapStudentStatus(student.Status),
                Notes = student.Notes,
                AverageScore = averageScore,
                ExamsCount = completedExams.Count,
                AttendanceRate = CalculateAttendanceRate(attendanceRecords),
                PhotoUrl = student.User?.PhotoUrl,

                // YENI
                TasksCount = tasks.Count,

                // YENI
                CompletedTasksCount = completedTasksCount,

                // YENI
                AbsentCount = absentCount,

                // YENI
                LateCount = lateCount,

                Exams = completedExams
                    .OrderByDescending(x => x.StartTime)
                    .Select(x => new StudentExamSummaryDto
                    {
                        StudentExamId = x.Id,
                        ExamId = x.ExamId,
                        ExamTitle = x.Exam?.Title ?? string.Empty,
                        SubjectName = x.Exam?.Subject?.Name ?? string.Empty,

                        // YENI
                        TeacherName = x.Exam?.Teacher?.FullName ?? string.Empty,

                        Score = x.Score,

                        // YENI
                        MaxScore = x.Exam?.TotalScore ?? 100,

                        IsCompleted = x.IsCompleted,
                        StartTime = AzerbaijanTimeHelper.ToBakuTime(x.StartTime),
                        EndTime = AzerbaijanTimeHelper.ToBakuTime(x.EndTime),

                        // YENI
                        ExamType = MapExamType(x.Exam?.Type),

                        // YENI
                        Note = null
                    })
                    .ToList(),

                Attendance = attendanceRecords
                    .OrderByDescending(x => x.AttendanceSession?.SessionDate)
                    .Select(x => new StudentAttendanceSummaryDto
                    {
                        AttendanceSessionId = x.AttendanceSessionId,
                        SessionDate = x.AttendanceSession != null
    ? AzerbaijanTimeHelper.ToBakuTime(x.AttendanceSession.SessionDate)
    : DateTime.MinValue,
                        SubjectName = x.AttendanceSession?.Subject?.Name ?? string.Empty,
                        TeacherName = x.AttendanceSession?.Teacher?.FullName ?? string.Empty,
                        Status = MapAttendanceStatus(x.Status),

                        // YENI
                        StartTime = x.AttendanceSession?.StartTime,

                        // YENI
                        EndTime = x.AttendanceSession?.EndTime,

                        // YENI
                        Note = x.Notes
                    })
                    .ToList(),

                Tasks = tasks
                    .OrderByDescending(x => x.AssignedDate)
                    .Select(MapToTaskDto)
                    .ToList()
            };
        }

        private StudentOptionDto MapToOptionDto(Student student)
        {
            var activeClass = student.StudentClasses?
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.JoinedAt)
                .FirstOrDefault();

            var completedExams = student.StudentExams?
                .Where(x => x.IsCompleted)
                .ToList() ?? new List<StudentExam>();

            return new StudentOptionDto
            {
                Id = student.Id,
                FullName = student.FullName,
                Email = student.User?.Email ?? string.Empty,
                PhotoUrl = student.User?.PhotoUrl,
                ClassName = activeClass?.ClassRoom?.Name,
                AverageScore = completedExams.Count > 0
                    ? Math.Round(completedExams.Average(x => x.Score), 2)
                    : null,
                Status = MapStudentStatus(student.Status)
            };
        }

        private static string MapStudentStatus(StudentStatus status)
        {
            return status switch
            {
                StudentStatus.Active => "Aktiv",
                StudentStatus.Passive => "Passiv",
                StudentStatus.Graduated => "Məzun",
                _ => "Aktiv"
            };
        }

        private static string MapAttendanceStatus(AttendanceStatus status)
        {
            return status switch
            {
                AttendanceStatus.Present => "Gəlib",
                AttendanceStatus.Absent => "Yoxdur",
                AttendanceStatus.Late => "Gecikib",
               
                _ => status.ToString()
            };
        }

        // YENI
        private static string MapGender(Gender? gender)
        {
            return gender switch
            {
                Gender.Male => "Kişi",
                Gender.Female => "Qadın",
                _ => "Bilinmir"
            };
        }

        // YENI
        private static string MapExamType(ExamType? type)
        {
            return type switch
            {
                ExamType.Quiz => "Quiz",
                ExamType.Midterm => "Midterm",
                ExamType.Final => "Final",
                ExamType.Practice => "Practice",
                _ => "Unknown"
            };
        }

        private static double CalculateAttendanceRate(List<AttendanceRecord> records)
        {
            if (records.Count == 0)
                return 0;

            var positiveCount = records.Count(x =>
                x.Status == AttendanceStatus.Present ||
                x.Status == AttendanceStatus.Late );

            return Math.Round((double)positiveCount / records.Count * 100, 2);
        }

        public async Task<List<StudentTaskDto>> GetTasksAsync(int studentId, CancellationToken cancellationToken = default)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId, cancellationToken);
            if (student == null)
                throw new KeyNotFoundException("Şagird tapılmadı.");

            var tasks = await _unitOfWork.StudentTasks.GetByStudentIdAsync(studentId, cancellationToken);

            return tasks
                .OrderByDescending(x => x.AssignedDate)
                .Select(MapToTaskDto)
                .ToList();
        }

        public async Task<StudentTaskDto> CreateTaskAsync(int studentId, CreateStudentTaskDto request, CancellationToken cancellationToken = default)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId, cancellationToken);
            if (student == null)
                throw new KeyNotFoundException("Şagird tapılmadı.");

            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Task title boş ola bilməz.");

            if (request.SubjectId.HasValue)
            {
                var subject = await _unitOfWork.Subjects.GetByIdAsync(request.SubjectId.Value, cancellationToken);
                if (subject == null)
                    throw new InvalidOperationException("Subject tapılmadı.");
            }

            if (request.TeacherId.HasValue)
            {
                var teacher = await _unitOfWork.Teachers.GetByIdAsync(request.TeacherId.Value, cancellationToken);
                if (teacher == null)
                    throw new InvalidOperationException("Teacher tapılmadı.");
            }

            var task = new ExamDomain.Entities.StudentTask
            {
                StudentId = studentId,
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                SubjectId = request.SubjectId,
                TeacherId = request.TeacherId,
                AssignedDate = AzerbaijanTimeHelper.FromBakuToUtc(request.AssignedDate),
                DueDate = AzerbaijanTimeHelper.FromBakuToUtc(request.DueDate),
                Status = (ExamDomain.Enum.StudentTaskStatus)request.Status,
                Score = request.Score,
                MaxScore = request.MaxScore,
                Link = request.Link?.Trim(),
                Note = request.Note?.Trim()
            };

            await _unitOfWork.StudentTasks.AddAsync(task, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var created = await _unitOfWork.StudentTasks.GetByIdAsync(task.Id, cancellationToken);
            return MapToTaskDto(created!);
        }

        public async Task<StudentTaskDto> UpdateTaskAsync(int studentId, UpdateStudentTaskDto request, CancellationToken cancellationToken = default)
        {
            var task = await _unitOfWork.StudentTasks.GetByIdAsync(request.Id, cancellationToken);

            if (task == null || task.StudentId != studentId)
                throw new KeyNotFoundException("Task tapılmadı.");

            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Task title boş ola bilməz.");

            if (request.SubjectId.HasValue)
            {
                var subject = await _unitOfWork.Subjects.GetByIdAsync(request.SubjectId.Value, cancellationToken);
                if (subject == null)
                    throw new InvalidOperationException("Subject tapılmadı.");
            }

            if (request.TeacherId.HasValue)
            {
                var teacher = await _unitOfWork.Teachers.GetByIdAsync(request.TeacherId.Value, cancellationToken);
                if (teacher == null)
                    throw new InvalidOperationException("Teacher tapılmadı.");
            }

            task.Title = request.Title.Trim();
            task.Description = request.Description?.Trim();
            task.SubjectId = request.SubjectId;
            task.TeacherId = request.TeacherId;
            task.AssignedDate = AzerbaijanTimeHelper.FromBakuToUtc(request.AssignedDate);
            task.DueDate = AzerbaijanTimeHelper.FromBakuToUtc(request.DueDate);
            task.Status = (ExamDomain.Enum.StudentTaskStatus)request.Status;
            task.Score = request.Score;
            task.MaxScore = request.MaxScore;
            task.Link = request.Link?.Trim();
            task.Note = request.Note?.Trim();

            _unitOfWork.StudentTasks.Update(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updated = await _unitOfWork.StudentTasks.GetByIdAsync(task.Id, cancellationToken);
            return MapToTaskDto(updated!);
        }

        public async Task DeleteTaskAsync(int studentId, int taskId, CancellationToken cancellationToken = default)
        {
            var task = await _unitOfWork.StudentTasks.GetByIdAsync(taskId, cancellationToken);

            if (task == null || task.StudentId != studentId)
                throw new KeyNotFoundException("Task tapılmadı.");

            _unitOfWork.StudentTasks.Remove(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<StudentExamReviewDto> GetExamReviewAsync(int studentId, int studentExamId, CancellationToken cancellationToken = default)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(studentId, cancellationToken);
            if (student == null)
                throw new KeyNotFoundException("Şagird tapılmadı.");

            var studentExam = await _unitOfWork.StudentExams.GetByIdWithAnswersAsync(studentExamId, cancellationToken);
            if (studentExam == null || studentExam.StudentId != studentId)
                throw new KeyNotFoundException("Student exam review tapılmadı.");

            var answers = await _unitOfWork.StudentAnswers.GetByStudentExamIdWithDetailsAsync(studentExamId, cancellationToken);

            return new StudentExamReviewDto
            {
                StudentExamId = studentExam.Id,
                ExamId = studentExam.ExamId,
                ExamTitle = studentExam.Exam?.Title ?? string.Empty,
                SubjectName = studentExam.Exam?.Subject?.Name ?? string.Empty,
                TeacherName = studentExam.Exam?.Teacher?.FullName ?? string.Empty,
                ExamDate = studentExam.StartTime,
                Score = studentExam.Score,
                Questions = answers
                    .OrderBy(x => x.ExamQuestion.OrderNumber)
                    .Select(x => new StudentExamReviewQuestionDto
                    {
                        Id = x.ExamQuestionId,
                        ExamId = studentExam.ExamId,
                        QuestionNo = x.ExamQuestion.OrderNumber,
                        Type = MapQuestionType(x.ExamQuestion.QuestionType),
                        QuestionText = x.ExamQuestion.QuestionText,
                        Options = x.ExamQuestion.Options
                            .OrderBy(o => o.OrderNumber)
                            .Select(o => new StudentExamReviewOptionDto
                            {
                                Id = o.Id,
                                Text = o.OptionText,
                                IsCorrect = o.IsCorrect
                            })
                            .ToList(),
                        CorrectAnswerText = BuildCorrectAnswerText(x),
                        StudentAnswerText = BuildStudentAnswerText(x),
                        AwardedScore = x.PointsAwarded,
                        MaxScore = x.ExamQuestion.Points,
                        TeacherFeedback = x.TeacherFeedback ?? string.Empty
                    })
                    .ToList()
            };
        }

        private StudentTaskDto MapToTaskDto(ExamDomain.Entities.StudentTask task)
        {
            return new StudentTaskDto
            {
                Id = task.Id,
                Title = task.Title,
                SubjectName = task.Subject?.Name ?? string.Empty,
                TeacherName = task.Teacher?.FullName ?? string.Empty,
                AssignedDate = AzerbaijanTimeHelper.ToBakuTime(task.AssignedDate),
                DueDate = AzerbaijanTimeHelper.ToBakuTime(task.DueDate),
                Status = MapStudentTaskStatus(task.Status),
                Score = task.Score,
                MaxScore = task.MaxScore,
                Link = task.Link,
                Note = task.Note,

                // YENI
                Description = task.Description
            };
        }

        private static string MapStudentTaskStatus(ExamDomain.Enum.StudentTaskStatus status)
        {
            return status switch
            {
                ExamDomain.Enum.StudentTaskStatus.Pending => "Pending",
                ExamDomain.Enum.StudentTaskStatus.Submitted => "Submitted",
                ExamDomain.Enum.StudentTaskStatus.Reviewed => "Reviewed",
                ExamDomain.Enum.StudentTaskStatus.Late => "Late",
                ExamDomain.Enum.StudentTaskStatus.Missing => "Missing",
                _ => "Pending"
            };
        }

        private static string MapQuestionType(ExamDomain.Enum.QuestionType type)
        {
            return type switch
            {
                ExamDomain.Enum.QuestionType.SingleChoice => "MultipleChoice",
                ExamDomain.Enum.QuestionType.MultipleChoice => "MultipleChoice",
                ExamDomain.Enum.QuestionType.OpenText => "OpenEnded",
                _ => type.ToString()
            };
        }

        private static string BuildCorrectAnswerText(ExamDomain.Entities.StudentAnswer answer)
        {
            if (answer.ExamQuestion.QuestionType == ExamDomain.Enum.QuestionType.OpenText)
                return "Açıq cavab";

            var correctOptions = answer.ExamQuestion.Options
                .Where(x => x.IsCorrect)
                .OrderBy(x => x.OrderNumber)
                .Select(x => x.OptionText)
                .ToList();

            return string.Join(", ", correctOptions);
        }
        private async Task<bool> HasBlockingRelationsAsync(Student student, CancellationToken cancellationToken)
        {
            var detailedStudent = student.StudentExams != null &&
                                  student.AttendanceRecords != null &&
                                  student.Tasks != null
                ? student
                : await _unitOfWork.Students.GetByIdWithDetailsAsync(student.Id, cancellationToken) ?? student;

            var hasStudentExams = detailedStudent.StudentExams?.Any() == true;
            var hasAttendanceRecords = detailedStudent.AttendanceRecords?.Any() == true;
            var hasTasks = detailedStudent.Tasks?.Any() == true;

            return hasStudentExams || hasAttendanceRecords || hasTasks;
        }
        private static string BuildStudentAnswerText(ExamDomain.Entities.StudentAnswer answer)
        {
            if (answer.ExamQuestion.QuestionType == ExamDomain.Enum.QuestionType.OpenText)
                return answer.AnswerText ?? string.Empty;

            if (answer.SelectedOptions?.Any() == true)
            {
                return string.Join(", ",
                    answer.SelectedOptions
                        .OrderBy(x => x.ExamOption.OrderNumber)
                        .Select(x => x.ExamOption.OptionText));
            }

            if (answer.SelectedOption != null)
                return answer.SelectedOption.OptionText;

            return string.Empty;
        }
    }
}