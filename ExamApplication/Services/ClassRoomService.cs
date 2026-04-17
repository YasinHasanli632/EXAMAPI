using ExamApplication.DTO.Class;
using ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Services;
using ExamDomain.Entities;
using ExamDomain.Enum;
using Microsoft.EntityFrameworkCore;

namespace ExamInfrastucture.Services
{
    public class ClassRoomService : IClassRoomService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClassRoomService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ClassListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var classes = await _unitOfWork.ClassRooms.GetAllWithDetailsAsync(cancellationToken);

            return classes
                .OrderBy(x => x.Grade)
                .ThenBy(x => x.Name)
                .Select(x =>
                {
                    var activeStudents = x.StudentClasses?.Where(sc => sc.IsActive).ToList() ?? new List<StudentClass>();
                    var activeAssignments = x.ClassTeacherSubjects?.Where(ct => ct.IsActive).ToList() ?? new List<ClassTeacherSubject>();
                    var studentExams = x.Exams?.SelectMany(e => e.StudentExams ?? new List<StudentExam>()).Where(se => se.IsCompleted).ToList()
                                      ?? new List<StudentExam>();

                    var avgScore = studentExams.Count > 0
                        ? Math.Round(studentExams.Average(se => se.Score), 2)
                        : 0;

                    return new ClassListItemDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        AcademicYear = x.AcademicYear ?? string.Empty,
                        Room = x.Room ?? string.Empty,
                        Status = x.IsActive ? "Aktiv" : "Passiv",
                        StudentCount = activeStudents.Count,
                        SubjectCount = activeAssignments.Select(a => a.SubjectId).Distinct().Count(),
                        TeacherCount = activeAssignments.Select(a => a.TeacherId).Distinct().Count(),
                        AverageScore = avgScore
                    };
                })
                .ToList();
        }

        public async Task<ClassDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var classRoom = await _unitOfWork.ClassRooms.GetByIdWithDetailsAsync(id, cancellationToken);
            if (classRoom == null)
                throw new KeyNotFoundException("Sinif tapılmadı.");

            var subjects = BuildSubjectOptions(classRoom);
            var teacherOptions = await GetTeacherOptionsInternalAsync(null, cancellationToken);
            var students = await BuildStudentOptionsByClassAsync(id, cancellationToken);
            var exams = BuildExamItems(classRoom);
            var rows = BuildTeacherSubjectRows(classRoom, subjects, teacherOptions);

            var avgScore = students
                .Where(x => x.AverageScore.HasValue)
                .Select(x => x.AverageScore!.Value)
                .DefaultIfEmpty(0)
                .Average();

            var avgAttendance = students
                .Where(x => x.AttendanceRate.HasValue)
                .Select(x => x.AttendanceRate!.Value)
                .DefaultIfEmpty(0)
                .Average();

            var topStudents = students
                .Where(x => x.AverageScore.HasValue)
                .OrderByDescending(x => x.AverageScore!.Value)
                .ThenBy(x => x.FullName)
                .Take(5)
                .Select(x => new ClassTopStudentDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email,
                    PhotoUrl = x.PhotoUrl,
                    AverageScore = x.AverageScore ?? 0
                })
                .ToList();

            return new ClassDetailDto
            {
                Id = classRoom.Id,
                Name = classRoom.Name,
                AcademicYear = classRoom.AcademicYear ?? string.Empty,
                Room = classRoom.Room ?? string.Empty,
                Description = classRoom.Description,
                Status = classRoom.IsActive ? "Aktiv" : "Passiv",
                MaxStudentCount = classRoom.MaxStudentCount,
                AverageScore = Math.Round(avgScore, 2),
                AttendanceRate = Math.Round(avgAttendance, 2),
                ExamCount = exams.Count,
                CreatedAt = classRoom.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                Students = students,
                TopStudents = topStudents,
                Subjects = subjects,
                Exams = exams.OrderByDescending(x => ParseDate(x.ExamDate)).ToList(),
                TeacherSubjectRows = rows
            };
        }

        public async Task<ClassDetailDto> CreateAsync(CreateClassDto request, CancellationToken cancellationToken = default)
        {
            request.SubjectIds ??= new List<int>();
            request.TeacherAssignments ??= new List<ClassTeacherAssignmentDto>();
            request.StudentIds ??= new List<int>();

            await ValidateCreateOrUpdateAsync(
                request.Name,
                null,
                request.SubjectIds,
                request.TeacherAssignments,
                request.StudentIds,
                cancellationToken);

            var entity = new ClassRoom
            {
                Name = request.Name.Trim(),
                Grade = ExtractGrade(request.Name),
                AcademicYear = request.AcademicYear?.Trim() ?? string.Empty,
                Room = string.IsNullOrWhiteSpace(request.Room) ? null : request.Room.Trim(),
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
                IsActive = string.Equals(request.Status, "Aktiv", StringComparison.OrdinalIgnoreCase),
                MaxStudentCount = request.MaxStudentCount
            };

            await _unitOfWork.ClassRooms.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await SyncTeacherAssignmentsInternalAsync(
                entity.Id,
                request.SubjectIds,
                request.TeacherAssignments,
                cancellationToken);

            await SyncStudentsInternalAsync(
                entity.Id,
                request.StudentIds,
                cancellationToken);

            return await GetByIdAsync(entity.Id, cancellationToken);
        }

        public async Task<ClassDetailDto> UpdateAsync(UpdateClassDto request, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.ClassRooms.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null)
                throw new KeyNotFoundException("Sinif tapılmadı.");

            request.SubjectIds ??= new List<int>();
            request.TeacherAssignments ??= new List<ClassTeacherAssignmentDto>();
            request.StudentIds ??= new List<int>();

            await ValidateCreateOrUpdateAsync(
                request.Name,
                request.Id,
                request.SubjectIds,
                request.TeacherAssignments,
                request.StudentIds,
                cancellationToken);

            entity.Name = request.Name.Trim();
            entity.Grade = ExtractGrade(request.Name);
            entity.AcademicYear = request.AcademicYear?.Trim() ?? string.Empty;
            entity.Room = string.IsNullOrWhiteSpace(request.Room) ? null : request.Room.Trim();
            entity.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
            entity.IsActive = string.Equals(request.Status, "Aktiv", StringComparison.OrdinalIgnoreCase);
            entity.MaxStudentCount = request.MaxStudentCount;

            _unitOfWork.ClassRooms.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await SyncTeacherAssignmentsInternalAsync(
                entity.Id,
                request.SubjectIds,
                request.TeacherAssignments,
                cancellationToken);

            await SyncStudentsInternalAsync(
                entity.Id,
                request.StudentIds,
                cancellationToken);

            return await GetByIdAsync(entity.Id, cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.ClassRooms.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                throw new KeyNotFoundException("Sinif tapılmadı.");

            var exams = await _unitOfWork.Exams.GetByClassRoomIdAsync(id, cancellationToken);
            if (exams.Any())
                throw new InvalidOperationException("Bu sinifə bağlı imtahanlar var. Əvvəl onları sil və ya ayır.");

            _unitOfWork.ClassRooms.Remove(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<ClassStudentOptionDto>> SearchStudentsAsync(string? search, CancellationToken cancellationToken = default)
        {
            var students = string.IsNullOrWhiteSpace(search)
                ? await _unitOfWork.Students.GetAllWithDetailsAsync(cancellationToken)
                : await _unitOfWork.Students.SearchByFullNameAsync(search.Trim(), cancellationToken);

            var studentIds = students.Select(x => x.Id).ToList();
            var studentClasses = await _unitOfWork.StudentClasses.GetByStudentIdsAsync(studentIds, cancellationToken);

            return students
                .OrderBy(x => x.FullName)
                .Select(student =>
                {
                    var currentClass = studentClasses
                        .Where(sc => sc.StudentId == student.Id && sc.IsActive)
                        .OrderByDescending(sc => sc.JoinedAt)
                        .FirstOrDefault();

                    var completedExams = student.StudentExams?.Where(se => se.IsCompleted).ToList() ?? new List<StudentExam>();
                    var attendance = student.AttendanceRecords ?? new List<AttendanceRecord>();

                    return new ClassStudentOptionDto
                    {
                        Id = student.Id,
                        FullName = student.FullName,
                        Email = student.User?.Email ?? string.Empty,
                        PhotoUrl = student.User?.PhotoUrl ?? string.Empty,
                        ClassName = currentClass?.ClassRoom?.Name,
                        AverageScore = completedExams.Count > 0 ? Math.Round(completedExams.Average(x => x.Score), 2) : null,
                        AttendanceRate = CalculateAttendanceRate(attendance),
                        Status = MapStudentStatus(student.Status)
                    };
                })
                .ToList();
        }

        public async Task<List<ClassTeacherOptionDto>> SearchTeachersAsync(string? search, CancellationToken cancellationToken = default)
        {
            return await GetTeacherOptionsInternalAsync(search, cancellationToken);
        }

        public async Task<List<ClassSubjectOptionDto>> GetSubjectOptionsAsync(CancellationToken cancellationToken = default)
        {
            var subjects = await _unitOfWork.Subjects.GetAllAsync(cancellationToken);

            return subjects
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new ClassSubjectOptionDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    Description = x.Description
                })
                .ToList();
        }

        private async Task ValidateCreateOrUpdateAsync(
            string name,
            int? currentId,
            List<int>? subjectIds,
            List<ClassTeacherAssignmentDto>? teacherAssignments,
            List<int>? studentIds,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Sinif adı boş ola bilməz.");

            subjectIds ??= new List<int>();
            teacherAssignments ??= new List<ClassTeacherAssignmentDto>();
            studentIds ??= new List<int>();

            var normalizedName = name.Trim();

            var exists = currentId.HasValue
                ? await _unitOfWork.ClassRooms.ExistsByNameAsync(normalizedName, currentId.Value, cancellationToken)
                : await _unitOfWork.ClassRooms.ExistsByNameAsync(normalizedName, cancellationToken);

            if (exists)
                throw new InvalidOperationException("Bu adda sinif artıq mövcuddur.");

            var normalizedSubjectIds = subjectIds
                .Where(x => x > 0)
                .ToList();

            var normalizedStudentIds = studentIds
                .Where(x => x > 0)
                .ToList();

            var duplicateSubjectExists = normalizedSubjectIds
                .GroupBy(x => x)
                .Any(g => g.Count() > 1);

            if (duplicateSubjectExists)
                throw new InvalidOperationException("Eyni fənn sinfə bir neçə dəfə əlavə edilə bilməz.");

            var duplicateStudentExists = normalizedStudentIds
                .GroupBy(x => x)
                .Any(g => g.Count() > 1);

            if (duplicateStudentExists)
                throw new InvalidOperationException("Eyni şagird sinfə bir neçə dəfə əlavə edilə bilməz.");

            var distinctSubjectIds = normalizedSubjectIds.Distinct().ToList();
            var distinctStudentIds = normalizedStudentIds.Distinct().ToList();

            var cleanedTeacherAssignments = teacherAssignments
                .Where(x => x != null && x.SubjectId > 0)
                .Select(x => new ClassTeacherAssignmentDto
                {
                    SubjectId = x.SubjectId,
                    TeacherId = x.TeacherId.HasValue && x.TeacherId.Value > 0 ? x.TeacherId.Value : null
                })
                .ToList();

            var duplicateTeacherAssignmentExists = cleanedTeacherAssignments
                .GroupBy(x => new { x.SubjectId, x.TeacherId })
                .Any(g => g.Count() > 1);

            if (duplicateTeacherAssignmentExists)
                throw new InvalidOperationException("Eyni fənn üçün eyni müəllim təyinatı bir neçə dəfə əlavə edilə bilməz.");

            var duplicateAssignmentSubjectExists = cleanedTeacherAssignments
                .Where(x => x.TeacherId.HasValue)
                .GroupBy(x => x.SubjectId)
                .Any(g => g.Count() > 1);

            if (duplicateAssignmentSubjectExists)
                throw new InvalidOperationException("Bir fənn üçün yalnız 1 müəllim təyin edilə bilər.");

            var assignmentSubjectIds = cleanedTeacherAssignments
                .Select(x => x.SubjectId)
                .Distinct()
                .ToList();

            var orphanAssignmentSubjectIds = assignmentSubjectIds
                .Where(subjectId => !distinctSubjectIds.Contains(subjectId))
                .ToList();

            if (orphanAssignmentSubjectIds.Any())
                throw new InvalidOperationException("Müəllim təyinatı verilən bütün fənlər subjectIds daxilində olmalıdır.");

            if (distinctStudentIds.Any())
            {
                var students = await _unitOfWork.Students.GetByIdsAsync(distinctStudentIds, cancellationToken);
                if (students.Count != distinctStudentIds.Count)
                    throw new InvalidOperationException("Şagirdlərdən bəzisi tapılmadı.");
            }

            if (distinctSubjectIds.Any())
            {
                var subjects = await _unitOfWork.Subjects.GetByIdsAsync(distinctSubjectIds, cancellationToken);
                if (subjects.Count != distinctSubjectIds.Count)
                    throw new InvalidOperationException("Fənlərdən bəzisi tapılmadı.");
            }

            var distinctTeacherIds = cleanedTeacherAssignments
                .Where(x => x.TeacherId.HasValue)
                .Select(x => x.TeacherId!.Value)
                .Distinct()
                .ToList();

            if (distinctTeacherIds.Any())
            {
                var allTeachers = await _unitOfWork.Teachers.GetAllAsync(cancellationToken);
                var selectedTeachers = allTeachers
                    .Where(x => distinctTeacherIds.Contains(x.Id))
                    .ToList();

                if (selectedTeachers.Count != distinctTeacherIds.Count)
                    throw new InvalidOperationException("Müəllimlərdən bəzisi tapılmadı.");

                var teacherSubjects = await _unitOfWork.TeacherSubjects.GetByTeacherIdsAsync(distinctTeacherIds, cancellationToken);

                foreach (var assignment in cleanedTeacherAssignments.Where(x => x.TeacherId.HasValue))
                {
                    var valid = teacherSubjects.Any(ts =>
                        ts.TeacherId == assignment.TeacherId!.Value &&
                        ts.SubjectId == assignment.SubjectId &&
                        ts.IsActive);

                    if (!valid)
                    {
                        throw new InvalidOperationException(
                            $"TeacherId={assignment.TeacherId} bu SubjectId={assignment.SubjectId} fənninə bağlı deyil.");
                    }
                }
            }
        }

        private async Task SyncTeacherAssignmentsInternalAsync(
            int classRoomId,
            List<int>? subjectIds,
            List<ClassTeacherAssignmentDto>? assignments,
            CancellationToken cancellationToken)
        {
            subjectIds ??= new List<int>();
            assignments ??= new List<ClassTeacherAssignmentDto>();

            var detail = await _unitOfWork.ClassRooms.GetByIdWithTeacherAssignmentsAsync(classRoomId, cancellationToken);
            if (detail == null)
                throw new KeyNotFoundException("Sinif tapılmadı.");

            detail.ClassTeacherSubjects ??= new List<ClassTeacherSubject>();

            var currentAssignments = detail.ClassTeacherSubjects.ToList();

            foreach (var oldItem in currentAssignments.Where(x => x.IsActive))
            {
                oldItem.IsActive = false;
            }

            var distinctSubjectIds = subjectIds
                .Where(x => x > 0)
                .Distinct()
                .ToList();

            var cleanedAssignments = assignments
                .Where(x => x != null && x.SubjectId > 0 && x.TeacherId.HasValue && x.TeacherId.Value > 0)
                .GroupBy(x => x.SubjectId)
                .Select(g => g.First())
                .ToList();

            foreach (var subjectId in distinctSubjectIds)
            {
                var assignment = cleanedAssignments.FirstOrDefault(x => x.SubjectId == subjectId);
                if (assignment == null)
                    continue;

                detail.ClassTeacherSubjects.Add(new ClassTeacherSubject
                {
                    ClassRoomId = classRoomId,
                    SubjectId = subjectId,
                    TeacherId = assignment.TeacherId!.Value,
                    IsActive = true
                });
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task SyncStudentsInternalAsync(
            int classRoomId,
            List<int>? studentIds,
            CancellationToken cancellationToken)
        {
            studentIds ??= new List<int>();

            var existing = await _unitOfWork.StudentClasses.GetByClassRoomIdAsync(classRoomId, cancellationToken);
            var wantedIds = studentIds
                .Where(x => x > 0)
                .Distinct()
                .ToHashSet();

            foreach (var item in existing.Where(x => x.IsActive && !wantedIds.Contains(x.StudentId)))
            {
                item.IsActive = false;
                item.LeftAt = DateTime.UtcNow;
                _unitOfWork.StudentClasses.Update(item);
            }

            var existingActiveIds = existing
                .Where(x => x.IsActive)
                .Select(x => x.StudentId)
                .ToHashSet();

            foreach (var studentId in wantedIds)
            {
                if (existingActiveIds.Contains(studentId))
                    continue;

                var oldRelations = await _unitOfWork.StudentClasses.GetActiveByStudentIdAsync(studentId, cancellationToken);
                foreach (var relation in oldRelations)
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
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<List<ClassTeacherOptionDto>> GetTeacherOptionsInternalAsync(string? search, CancellationToken cancellationToken)
        {
            var teachers = await _unitOfWork.Teachers.GetAllAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLowerInvariant();
                teachers = teachers
                    .Where(t =>
                        t.FullName.ToLower().Contains(term) ||
                        (t.User?.Email ?? string.Empty).ToLower().Contains(term) ||
                        (t.Specialization ?? string.Empty).ToLower().Contains(term))
                    .ToList();
            }

            var teacherIds = teachers.Select(x => x.Id).ToList();
            var teacherSubjects = await _unitOfWork.TeacherSubjects.GetByTeacherIdsAsync(teacherIds, cancellationToken);

            return teachers
                .OrderBy(x => x.FullName)
                .Select(teacher =>
                {
                    var subjects = teacherSubjects
                        .Where(ts => ts.TeacherId == teacher.Id && ts.IsActive)
                        .ToList();

                    return new ClassTeacherOptionDto
                    {
                        Id = teacher.Id,
                        FullName = teacher.FullName,
                        Email = teacher.User?.Email ?? string.Empty,
                        PhotoUrl = teacher.User?.PhotoUrl ?? string.Empty,
                        SubjectIds = subjects.Select(x => x.SubjectId).Distinct().ToList(),
                        SubjectNames = subjects
                            .Where(x => x.Subject != null)
                            .Select(x => x.Subject.Name)
                            .Distinct()
                            .OrderBy(x => x)
                            .ToList(),
                        Status = MapTeacherStatus(teacher.Status)
                    };
                })
                .ToList();
        }

        private async Task<List<ClassStudentOptionDto>> BuildStudentOptionsByClassAsync(int classRoomId, CancellationToken cancellationToken)
        {
            var students = await _unitOfWork.Students.GetStudentsByClassRoomIdAsync(classRoomId, cancellationToken);

            return students
                .OrderBy(x => x.FullName)
                .Select(student =>
                {
                    var completedExams = student.StudentExams?.Where(se => se.IsCompleted).ToList() ?? new List<StudentExam>();
                    var attendance = student.AttendanceRecords ?? new List<AttendanceRecord>();
                    var activeClass = student.StudentClasses?.FirstOrDefault(x => x.IsActive)?.ClassRoom?.Name;

                    return new ClassStudentOptionDto
                    {
                        Id = student.Id,
                        FullName = student.FullName,
                        Email = student.User?.Email ?? string.Empty,
                        PhotoUrl = student.User?.PhotoUrl ?? string.Empty,
                        ClassName = activeClass,
                        AverageScore = completedExams.Count > 0 ? Math.Round(completedExams.Average(x => x.Score), 2) : null,
                        AttendanceRate = CalculateAttendanceRate(attendance),
                        Status = MapStudentStatus(student.Status)
                    };
                })
                .ToList();
        }

        private List<ClassSubjectOptionDto> BuildSubjectOptions(ClassRoom classRoom)
        {
            return (classRoom.ClassTeacherSubjects ?? new List<ClassTeacherSubject>())
                .Where(x => x.IsActive && x.Subject != null)
                .GroupBy(x => x.SubjectId)
                .Select(g =>
                {
                    var subject = g.First().Subject;
                    return new ClassSubjectOptionDto
                    {
                        Id = subject.Id,
                        Name = subject.Name,
                        Code = subject.Code,
                        Description = subject.Description
                    };
                })
                .OrderBy(x => x.Name)
                .ToList();
        }

        private List<ClassExamItemDto> BuildExamItems(ClassRoom classRoom)
        {
            return (classRoom.Exams ?? new List<Exam>())
                .OrderByDescending(x => x.StartTime)
                .Select(x => new ClassExamItemDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    SubjectId = x.SubjectId,
                    SubjectName = x.Subject?.Name ?? string.Empty,
                    ExamDate = x.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    DurationMinutes = x.DurationMinutes,
                    TotalScore = x.TotalScore ?? 0,
                    Status = MapExamStatus(x.Status)
                })
                .ToList();
        }

        private List<ClassTeacherSubjectRowDto> BuildTeacherSubjectRows(
            ClassRoom classRoom,
            List<ClassSubjectOptionDto> subjects,
            List<ClassTeacherOptionDto> teacherOptions)
        {
            var activeAssignments = classRoom.ClassTeacherSubjects?.Where(x => x.IsActive).ToList()
                                  ?? new List<ClassTeacherSubject>();

            return subjects
                .Select(subject =>
                {
                    var assigned = activeAssignments.FirstOrDefault(x => x.SubjectId == subject.Id);

                    return new ClassTeacherSubjectRowDto
                    {
                        SubjectId = subject.Id,
                        SubjectName = subject.Name,
                        SubjectCode = subject.Code,
                        AssignedTeacherId = assigned?.TeacherId,
                        AssignedTeacherName = assigned?.Teacher?.FullName ?? "Təyin edilməyib",
                        AvailableTeachers = teacherOptions
                            .Where(t => t.SubjectIds.Contains(subject.Id))
                            .OrderBy(t => t.FullName)
                            .ToList()
                    };
                })
                .ToList();
        }

        private static int ExtractGrade(string className)
        {
            if (string.IsNullOrWhiteSpace(className))
                return 0;

            var digits = new string(className.Where(char.IsDigit).ToArray());
            return int.TryParse(digits, out var grade) ? grade : 0;
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

        private static string MapTeacherStatus(TeacherStatus status)
        {
            return status switch
            {
                TeacherStatus.Active => "Aktiv",
                TeacherStatus.Passive => "Passiv",
                TeacherStatus.OnLeave => "Məzuniyyət",
                _ => "Aktiv"
            };
        }

        private static string MapExamStatus(ExamStatus status)
        {
            return status.ToString();
        }

        private static double? CalculateAttendanceRate(IEnumerable<AttendanceRecord>? records)
        {
            var list = records?.ToList() ?? new List<AttendanceRecord>();
            if (list.Count == 0)
                return null;

            var positive = list.Count(x =>
                x.Status == AttendanceStatus.Present ||
                x.Status == AttendanceStatus.Late);

            return Math.Round((double)positive / list.Count * 100, 2);
        }

        private static DateTime ParseDate(string value)
        {
            return DateTime.TryParse(value, out var date) ? date : DateTime.MinValue;
        }
    }
}