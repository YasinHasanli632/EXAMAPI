using ExamApplication.DTO.Teacher;
using ExamApplication.Helper;
using ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Services;
using ExamDomain.Entities;
using ExamDomain.Enum;

namespace ExamApplication.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService; // YENI

        public TeacherService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService) // YENI
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService; // YENI
        }

        // Yeni teacher yaradır
        public async Task<TeacherDto> CreateAsync(CreateTeacherDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new Exception("Sorğu boş ola bilməz.");

            if (request.UserId <= 0)
                throw new Exception("UserId düzgün deyil.");

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                throw new Exception("User tapılmadı.");

            var existingTeacher = await _unitOfWork.Teachers.GetByUserIdAsync(request.UserId, cancellationToken);
            if (existingTeacher != null)
                throw new Exception("Bu user üçün artıq teacher mövcuddur.");

            var teacher = new Teacher
            {
                UserId = request.UserId,
                FullName = !string.IsNullOrWhiteSpace(request.FullName)
                    ? request.FullName.Trim()
                    : $"{user.FirstName} {user.LastName}".Trim(),
                Department = request.Department?.Trim() ?? string.Empty,
                Specialization = string.IsNullOrWhiteSpace(request.Specialization)
                    ? request.Department?.Trim()
                    : request.Specialization.Trim(),
                Status = request.Status
            };

            await _unitOfWork.Teachers.AddAsync(teacher, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var createdTeacher = await _unitOfWork.Teachers.GetByIdWithDetailsAsync(teacher.Id, cancellationToken);
            if (createdTeacher == null)
                throw new Exception("Yaradılan teacher tapılmadı.");

            return MapTeacherDto(createdTeacher);
        }

        // Mövcud teacher-i yeniləyir
        public async Task<TeacherDto> UpdateAsync(UpdateTeacherDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new Exception("Sorğu boş ola bilməz.");

            if (request.Id <= 0)
                throw new Exception("Teacher Id düzgün deyil.");

            var teacher = await _unitOfWork.Teachers.GetByIdAsync(request.Id, cancellationToken);
            if (teacher == null)
                throw new Exception("Teacher tapılmadı.");

            if (!string.IsNullOrWhiteSpace(request.FullName))
                teacher.FullName = request.FullName.Trim();

            if (!string.IsNullOrWhiteSpace(request.Department))
                teacher.Department = request.Department.Trim();

            teacher.Specialization = string.IsNullOrWhiteSpace(request.Specialization)
                ? teacher.Specialization
                : request.Specialization.Trim();

            if (request.Status.HasValue)
                teacher.Status = request.Status.Value;

            _unitOfWork.Teachers.Update(teacher);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updatedTeacher = await _unitOfWork.Teachers.GetByIdWithDetailsAsync(teacher.Id, cancellationToken);
            if (updatedTeacher == null)
                throw new Exception("Yenilənmiş teacher tapılmadı.");

            return MapTeacherDto(updatedTeacher);
        }

        // Id-yə görə teacher gətirir
        public async Task<TeacherDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new Exception("Teacher Id düzgün deyil.");

            var teacher = await _unitOfWork.Teachers.GetByIdWithDetailsAsync(id, cancellationToken);
            if (teacher == null)
                throw new Exception("Teacher tapılmadı.");

            return MapTeacherDto(teacher);
        }

        // UserId-yə görə teacher gətirir
        public async Task<TeacherDto> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0)
                throw new Exception("UserId düzgün deyil.");

            var teacher = await _unitOfWork.Teachers.GetByUserIdWithDetailsAsync(userId, cancellationToken);
            if (teacher == null)
                throw new Exception("Teacher tapılmadı.");

            return MapTeacherDto(teacher);
        }

        // Id-yə görə teacher detail gətirir
        public async Task<TeacherDetailsDto> GetDetailsByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new Exception("Teacher Id düzgün deyil.");

            var teacher = await _unitOfWork.Teachers.GetByIdWithDetailsAsync(id, cancellationToken);
            if (teacher == null)
                throw new Exception("Teacher tapılmadı.");

            return await BuildTeacherDetailsDtoAsync(teacher, cancellationToken);
        }

        // Bütün teacher-ləri gətirir
        public async Task<List<TeacherDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var teachers = await _unitOfWork.Teachers.GetAllWithDetailsAsync(cancellationToken);

            return teachers
                .OrderBy(x => x.FullName)
                .Select(MapTeacherDto)
                .ToList();
        }

        // Teacher-ə subject bağlayır
        public async Task AssignSubjectAsync(AssignSubjectToTeacherDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new Exception("Sorğu boş ola bilməz.");

            if (request.TeacherId <= 0 || request.SubjectId <= 0)
                throw new Exception("TeacherId və SubjectId düzgün deyil.");

            var teacher = await _unitOfWork.Teachers.GetByIdAsync(request.TeacherId, cancellationToken);
            if (teacher == null)
                throw new Exception("Teacher tapılmadı.");

            var subject = await _unitOfWork.Subjects.GetByIdAsync(request.SubjectId, cancellationToken);
            if (subject == null)
                throw new Exception("Subject tapılmadı.");

            var exists = await _unitOfWork.TeacherSubjects.ExistsAsync(request.TeacherId, request.SubjectId, cancellationToken);
            if (exists)
                throw new Exception("Bu subject artıq teacher-ə bağlıdır.");

            var teacherSubject = new TeacherSubject
            {
                TeacherId = request.TeacherId,
                SubjectId = request.SubjectId,
                IsActive = true
            };

            await _unitOfWork.TeacherSubjects.AddAsync(teacherSubject, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Teacher-dən subject çıxarır
        public async Task RemoveSubjectAsync(RemoveSubjectFromTeacherDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new Exception("Sorğu boş ola bilməz.");

            if (request.TeacherId <= 0 || request.SubjectId <= 0)
                throw new Exception("TeacherId və SubjectId düzgün deyil.");

            var teacherSubject = await _unitOfWork.TeacherSubjects.GetByTeacherAndSubjectAsync(
                request.TeacherId,
                request.SubjectId,
                cancellationToken);

            if (teacherSubject == null)
                throw new Exception("Teacher-subject əlaqəsi tapılmadı.");

            var classAssignments = await _unitOfWork.ClassTeacherSubjects.GetByTeacherIdAsync(request.TeacherId, cancellationToken);
            var relatedAssignments = classAssignments
                .Where(x => x.SubjectId == request.SubjectId)
                .ToList();

            if (relatedAssignments.Any())
            {
                _unitOfWork.ClassTeacherSubjects.RemoveRange(relatedAssignments);
            }

            _unitOfWork.TeacherSubjects.Remove(teacherSubject);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Teacher-ə class bağlayır
        public async Task AssignClassRoomAsync(AssignClassRoomToTeacherDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new Exception("Sorğu boş ola bilməz.");

            if (request.TeacherId <= 0 || request.ClassRoomId <= 0)
                throw new Exception("TeacherId və ClassRoomId düzgün deyil.");

            if (request.SubjectId <= 0)
                throw new Exception("SubjectId düzgün deyil.");

            var teacher = await _unitOfWork.Teachers.GetByIdAsync(request.TeacherId, cancellationToken);
            if (teacher == null)
                throw new Exception("Teacher tapılmadı.");

            var classRoom = await _unitOfWork.ClassRooms.GetByIdAsync(request.ClassRoomId, cancellationToken);
            if (classRoom == null)
                throw new Exception("ClassRoom tapılmadı.");

            var subject = await _unitOfWork.Subjects.GetByIdAsync(request.SubjectId, cancellationToken);
            if (subject == null)
                throw new Exception("Subject tapılmadı.");

            var teacherSubjectExists = await _unitOfWork.TeacherSubjects.ExistsAsync(
                request.TeacherId,
                request.SubjectId,
                cancellationToken);

            if (!teacherSubjectExists)
                throw new Exception("Teacher bu subject-ə bağlı deyil. Əvvəlcə subject assignment edin.");

            var exists = await _unitOfWork.ClassTeacherSubjects.ExistsAsync(
                request.ClassRoomId,
                request.TeacherId,
                request.SubjectId,
                cancellationToken);

            if (exists)
                throw new Exception("Bu class-room assignment artıq mövcuddur.");

            var classTeacherSubject = new ClassTeacherSubject
            {
                TeacherId = request.TeacherId,
                ClassRoomId = request.ClassRoomId,
                SubjectId = request.SubjectId
            };

            await _unitOfWork.ClassTeacherSubjects.AddAsync(classTeacherSubject, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Teacher-dən class çıxarır
        public async Task RemoveClassRoomAsync(RemoveClassRoomFromTeacherDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new Exception("Sorğu boş ola bilməz.");

            if (request.TeacherId <= 0 || request.ClassRoomId <= 0)
                throw new Exception("TeacherId və ClassRoomId düzgün deyil.");

            if (request.SubjectId <= 0)
                throw new Exception("SubjectId düzgün deyil.");

            var classAssignments = await _unitOfWork.ClassTeacherSubjects.GetByTeacherIdAsync(request.TeacherId, cancellationToken);

            var assignment = classAssignments.FirstOrDefault(x =>
                x.ClassRoomId == request.ClassRoomId &&
                x.SubjectId == request.SubjectId);

            if (assignment == null)
                throw new Exception("Teacher-classroom əlaqəsi tapılmadı.");

            _unitOfWork.ClassTeacherSubjects.Remove(assignment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Teacher-in bütün subject-lərini gətirir
        public async Task<List<TeacherSubjectDto>> GetSubjectsByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default)
        {
            if (teacherId <= 0)
                throw new Exception("TeacherId düzgün deyil.");

            var teacher = await _unitOfWork.Teachers.GetByIdAsync(teacherId, cancellationToken);
            if (teacher == null)
                throw new Exception("Teacher tapılmadı.");

            var teacherSubjects = await _unitOfWork.TeacherSubjects.GetByTeacherIdAsync(teacherId, cancellationToken);

            return teacherSubjects
                .OrderBy(x => x.Subject?.Name)
                .Select(x => new TeacherSubjectDto
                {
                    Id = x.Id,
                    SubjectId = x.SubjectId,
                    SubjectName = x.Subject?.Name ?? string.Empty,
                    SubjectCode = x.Subject?.Code,
                    IsActive = x.IsActive
                })
                .ToList();
        }
        // YENI
        public async Task<TeacherDetailsDto> GetMeAsync(CancellationToken cancellationToken = default)
        {
            var currentUser = _currentUserService.GetCurrentUser();
            if (currentUser == null)
                throw new UnauthorizedAccessException("Cari istifadəçi tapılmadı.");

            return await GetDetailsByUserIdAsync(currentUser.UserId, cancellationToken);
        }
        // YENI
        public async Task<TeacherDashboardDto> GetMyDashboardAsync(CancellationToken cancellationToken = default)
        {
            var currentUser = _currentUserService.GetCurrentUser();
            if (currentUser == null)
                throw new UnauthorizedAccessException("Cari istifadəçi tapılmadı.");

            var teacher = await _unitOfWork.Teachers.GetByUserIdWithFullDashboardAsync(currentUser.UserId, cancellationToken);
            if (teacher == null)
                throw new KeyNotFoundException("Müəllim tapılmadı.");

            var classAssignments = teacher.ClassTeacherSubjects?.ToList() ?? new List<ExamDomain.Entities.ClassTeacherSubject>();
            var classRoomIds = classAssignments.Select(x => x.ClassRoomId).Distinct().ToList();

            var classes = new List<TeacherMyClassRoomListItemDto>();

            foreach (var classRoomId in classRoomIds)
            {
                var classRoom = await _unitOfWork.ClassRooms.GetByIdWithDetailsAsync(classRoomId, cancellationToken);
                if (classRoom == null)
                    continue;

                var students = classRoom.StudentClasses?
                    .Where(x => x.IsActive && x.Student != null)
                    .Select(x => x.Student!)
                    .DistinctBy(x => x.Id)
                    .ToList() ?? new List<ExamDomain.Entities.Student>();

                var exams = classRoom.Exams?
                    .Where(x => x.TeacherId == teacher.Id)
                    .ToList() ?? new List<ExamDomain.Entities.Exam>();

                var topStudent = students
                    .Select(s => new
                    {
                        Student = s,
                        Average = s.StudentExams != null && s.StudentExams.Any()
                            ? s.StudentExams.Average(se => se.Score)
                            : 0
                    })
                    .OrderByDescending(x => x.Average)
                    .FirstOrDefault();

                var subjectNames = classAssignments
                    .Where(x => x.ClassRoomId == classRoomId && x.Subject != null)
                    .Select(x => x.Subject!.Name)
                    .Distinct()
                    .ToList();

                classes.Add(new TeacherMyClassRoomListItemDto
                {
                    ClassRoomId = classRoom.Id,
                    ClassRoomName = classRoom.Name,
                    StudentCount = students.Count,
                    ExamCount = exams.Count,
                    AverageScore = students.Any() && students.SelectMany(x => x.StudentExams ?? new List<ExamDomain.Entities.StudentExam>()).Any()
                        ? Math.Round(students.SelectMany(x => x.StudentExams!).Average(x => x.Score), 2)
                        : 0,
                    AttendanceRate = students.Any()
                        ? Math.Round(students
                            .Where(x => x.AttendanceRecords != null && x.AttendanceRecords.Any())
                            .Select(x =>
                            {
                                var records = x.AttendanceRecords!;
                                var total = records.Count;
                                if (total == 0) return 0d;
                                var present = records.Count(r =>
                                    r.Status == ExamDomain.Enum.AttendanceStatus.Present ||
                                    r.Status == ExamDomain.Enum.AttendanceStatus.Late );
                                return (double)present / total * 100d;
                            }).DefaultIfEmpty(0d).Average(), 2)
                        : 0,
                    SubjectNames = subjectNames,
                    TopStudentName = topStudent?.Student.FullName,
                    TopStudentScore = topStudent != null ? Math.Round(topStudent.Average, 2) : null
                });
            }

            var unreadCount = await _unitOfWork.Notifications.GetUnreadCountByUserIdAsync(currentUser.UserId, cancellationToken);
            var tasks = await _unitOfWork.TeacherTasks.GetByTeacherIdAsync(teacher.Id, cancellationToken);

            return new TeacherDashboardDto
            {
                TeacherId = teacher.Id,
                FullName = teacher.FullName,
                TotalClasses = classes.Count,
                TotalStudents = classes.Sum(x => x.StudentCount),
                TotalExams = classes.Sum(x => x.ExamCount),
                TotalTasks = tasks.Count,
                PendingTasks = tasks.Count(x => !x.IsCompleted),
                UnreadNotificationsCount = unreadCount,
                Classes = classes
            };
        }
        // YENI
        public async Task<List<TeacherMyClassRoomListItemDto>> GetMyClassRoomsAsync(CancellationToken cancellationToken = default)
        {
            var dashboard = await GetMyDashboardAsync(cancellationToken);
            return dashboard.Classes;
        }
        // YENI
        // YENI
        public async Task<ExamApplication.DTO.Class.ClassDetailDto> GetMyClassRoomDetailsAsync(int classRoomId, CancellationToken cancellationToken = default)
        {
            var currentUser = _currentUserService.GetCurrentUser();
            if (currentUser == null)
                throw new UnauthorizedAccessException("Cari istifadəçi tapılmadı.");

            var teacher = await _unitOfWork.Teachers.GetByUserIdAsync(currentUser.UserId, cancellationToken);
            if (teacher == null)
                throw new KeyNotFoundException("Müəllim tapılmadı.");

            var hasAccess = await _unitOfWork.ClassTeacherSubjects.GetByTeacherIdAsync(teacher.Id, cancellationToken);
            if (!hasAccess.Any(x => x.ClassRoomId == classRoomId))
                throw new UnauthorizedAccessException("Bu sinfə giriş icazəniz yoxdur.");

            var classRoom = await _unitOfWork.ClassRooms.GetByIdWithDetailsAsync(classRoomId, cancellationToken);
            if (classRoom == null)
                throw new KeyNotFoundException("Sinif tapılmadı.");

            // Burada istəsən mövcud ClassRoomService mapping məntiqini ayrıca private helper-ə çıxarıb reuse et.
            // Ən düzgün yol budur.
            throw new NotImplementedException("Burada mövcud ClassRoomService detail mapping helper-i reuse olunmalıdır.");
        }
        // Teacher-in bütün class-larını gətirir
        public async Task<List<TeacherClassRoomDto>> GetClassRoomsByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default)
        {
            if (teacherId <= 0)
                throw new Exception("TeacherId düzgün deyil.");

            var teacher = await _unitOfWork.Teachers.GetByIdAsync(teacherId, cancellationToken);
            if (teacher == null)
                throw new Exception("Teacher tapılmadı.");

            var classAssignments = await _unitOfWork.ClassTeacherSubjects.GetByTeacherIdAsync(teacherId, cancellationToken);

            return classAssignments
                .OrderBy(x => x.ClassRoom?.Grade)
                .ThenBy(x => x.ClassRoom?.Name)
                .ThenBy(x => x.Subject?.Name)
                .Select(x => new TeacherClassRoomDto
                {
                    Id = x.Id,
                    ClassRoomId = x.ClassRoomId,
                    ClassRoomName = x.ClassRoom?.Name ?? string.Empty,
                    Grade = x.ClassRoom?.Grade ?? 0,
                    SubjectId = x.SubjectId,
                    SubjectName = x.Subject?.Name,
                    IsActive = true
                })
                .ToList();
        }

        // Teacher-i silir
        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new Exception("Teacher Id düzgün deyil.");

            var teacher = await _unitOfWork.Teachers.GetByIdAsync(id, cancellationToken);
            if (teacher == null)
                throw new Exception("Teacher tapılmadı.");

            var teacherSubjects = await _unitOfWork.TeacherSubjects.GetByTeacherIdAsync(id, cancellationToken);
            foreach (var item in teacherSubjects)
            {
                _unitOfWork.TeacherSubjects.Remove(item);
            }

            var classAssignments = await _unitOfWork.ClassTeacherSubjects.GetByTeacherIdAsync(id, cancellationToken);
            if (classAssignments.Any())
            {
                _unitOfWork.ClassTeacherSubjects.RemoveRange(classAssignments);
            }

            _unitOfWork.Teachers.Remove(teacher);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<TeacherDetailsDto> GetDetailsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId <= 0)
                throw new Exception("UserId düzgün deyil.");

            var teacher = await _unitOfWork.Teachers.GetByUserIdWithDetailsAsync(userId, cancellationToken);
            if (teacher == null)
                throw new Exception("Teacher tapılmadı.");

            return await BuildTeacherDetailsDtoAsync(teacher, cancellationToken);
        }

        public async Task<List<TeacherDetailsDto>> GetAllDetailsAsync(CancellationToken cancellationToken = default)
        {
            var teachers = await _unitOfWork.Teachers.GetAllWithDetailsAsync(cancellationToken);
            var result = new List<TeacherDetailsDto>();

            foreach (var teacher in teachers.OrderBy(x => x.FullName))
            {
                result.Add(await BuildTeacherDetailsDtoAsync(teacher, cancellationToken));
            }

            return result;
        }

        public async Task ChangeStatusAsync(ChangeTeacherStatusDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new Exception("Sorğu boş ola bilməz.");

            if (request.TeacherId <= 0)
                throw new Exception("TeacherId düzgün deyil.");

            var teacher = await _unitOfWork.Teachers.GetByIdAsync(request.TeacherId, cancellationToken);
            if (teacher == null)
                throw new Exception("Teacher tapılmadı.");

            teacher.Status = request.Status;
            _unitOfWork.Teachers.Update(teacher);

            var user = await _unitOfWork.Users.GetByIdAsync(teacher.UserId, cancellationToken);
            if (user != null)
            {
                user.IsActive = request.Status != TeacherStatus.Passive;
                _unitOfWork.Users.Update(user);
            }

            if (request.Status == TeacherStatus.Passive)
            {
                var classAssignments = await _unitOfWork.ClassTeacherSubjects.GetByTeacherIdAsync(request.TeacherId, cancellationToken);
                foreach (var item in classAssignments)
                {
                    // ayrıca IsActive field yoxdursa heç nə etmə
                    // əgər entity-də IsActive varsa, aç bunu:
                    // item.IsActive = false;
                }

                var teacherSubjects = await _unitOfWork.TeacherSubjects.GetByTeacherIdAsync(request.TeacherId, cancellationToken);
                foreach (var item in teacherSubjects.Where(x => x.IsActive))
                {
                    item.IsActive = false;
                    _unitOfWork.TeacherSubjects.Update(item);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task SyncSubjectsAsync(SyncTeacherSubjectsDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new Exception("Sorğu boş ola bilməz.");

            if (request.TeacherId <= 0)
                throw new Exception("TeacherId düzgün deyil.");

            request.SubjectIds ??= new List<int>();

            var teacher = await _unitOfWork.Teachers.GetByIdAsync(request.TeacherId, cancellationToken);
            if (teacher == null)
                throw new Exception("Teacher tapılmadı.");

            var normalizedIds = request.SubjectIds
                .Where(x => x > 0)
                .Distinct()
                .ToList();

            if (normalizedIds.Count > 0)
            {
                var subjects = await _unitOfWork.Subjects.GetByIdsAsync(normalizedIds, cancellationToken);
                var foundIds = subjects.Select(x => x.Id).ToHashSet();

                var missingIds = normalizedIds
                    .Where(x => !foundIds.Contains(x))
                    .ToList();

                if (missingIds.Any())
                    throw new Exception($"Bəzi subject-lər tapılmadı: {string.Join(", ", missingIds)}");
            }

            var currentItems = await _unitOfWork.TeacherSubjects.GetByTeacherIdAsync(request.TeacherId, cancellationToken);

            var currentIds = currentItems.Select(x => x.SubjectId).ToHashSet();
            var newIds = normalizedIds.ToHashSet();

            var removeItems = currentItems
                .Where(x => !newIds.Contains(x.SubjectId))
                .ToList();

            if (removeItems.Any())
            {
                var currentAssignments = await _unitOfWork.ClassTeacherSubjects.GetByTeacherIdAsync(request.TeacherId, cancellationToken);
                var removedSubjectIds = removeItems.Select(x => x.SubjectId).ToHashSet();

                var relatedAssignments = currentAssignments
                    .Where(x => removedSubjectIds.Contains(x.SubjectId))
                    .ToList();

                if (relatedAssignments.Any())
                {
                    _unitOfWork.ClassTeacherSubjects.RemoveRange(relatedAssignments);
                }
            }

            foreach (var item in removeItems)
            {
                _unitOfWork.TeacherSubjects.Remove(item);
            }

            var addIds = newIds.Where(x => !currentIds.Contains(x)).ToList();

            foreach (var subjectId in addIds)
            {
                await _unitOfWork.TeacherSubjects.AddAsync(new TeacherSubject
                {
                    TeacherId = request.TeacherId,
                    SubjectId = subjectId,
                    IsActive = true
                }, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task SyncClassRoomsAsync(SyncTeacherClassRoomsDto request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new Exception("Sorğu boş ola bilməz.");

            if (request.TeacherId <= 0)
                throw new Exception("TeacherId düzgün deyil.");

            request.Assignments ??= new List<TeacherClassRoomAssignmentDto>();

            var teacher = await _unitOfWork.Teachers.GetByIdAsync(request.TeacherId, cancellationToken);
            if (teacher == null)
                throw new Exception("Teacher tapılmadı.");

            var normalizedAssignments = request.Assignments
                .Where(x => x.ClassRoomId > 0 && x.SubjectId > 0)
                .GroupBy(x => new { x.ClassRoomId, x.SubjectId })
                .Select(x => x.First())
                .ToList();

            var teacherSubjectIds = (await _unitOfWork.TeacherSubjects.GetByTeacherIdAsync(request.TeacherId, cancellationToken))
                .Where(x => x.IsActive)
                .Select(x => x.SubjectId)
                .ToHashSet();

            foreach (var assignment in normalizedAssignments)
            {
                if (!teacherSubjectIds.Contains(assignment.SubjectId))
                    throw new Exception($"Teacher {assignment.SubjectId} id-li subject-ə bağlı deyil.");
            }

            var classRoomIds = normalizedAssignments
                .Select(x => x.ClassRoomId)
                .Distinct()
                .ToList();

            foreach (var classRoomId in classRoomIds)
            {
                var classRoom = await _unitOfWork.ClassRooms.GetByIdAsync(classRoomId, cancellationToken);
                if (classRoom == null)
                    throw new Exception($"Sinif tapılmadı. Id: {classRoomId}");
            }

            var subjectIds = normalizedAssignments
                .Select(x => x.SubjectId)
                .Distinct()
                .ToList();

            foreach (var subjectId in subjectIds)
            {
                var subject = await _unitOfWork.Subjects.GetByIdAsync(subjectId, cancellationToken);
                if (subject == null)
                    throw new Exception($"Fənn tapılmadı. Id: {subjectId}");
            }

            var currentItems = await _unitOfWork.ClassTeacherSubjects.GetByTeacherIdAsync(request.TeacherId, cancellationToken);

            var currentKeys = currentItems
                .Select(x => $"{x.ClassRoomId}-{x.SubjectId}")
                .ToHashSet();

            var newKeys = normalizedAssignments
                .Select(x => $"{x.ClassRoomId}-{x.SubjectId}")
                .ToHashSet();

            var removeItems = currentItems
                .Where(x => !newKeys.Contains($"{x.ClassRoomId}-{x.SubjectId}"))
                .ToList();

            if (removeItems.Any())
            {
                _unitOfWork.ClassTeacherSubjects.RemoveRange(removeItems);
            }

            var addItems = normalizedAssignments
                .Where(x => !currentKeys.Contains($"{x.ClassRoomId}-{x.SubjectId}"))
                .ToList();

            foreach (var item in addItems)
            {
                await _unitOfWork.ClassTeacherSubjects.AddAsync(new ClassTeacherSubject
                {
                    TeacherId = request.TeacherId,
                    ClassRoomId = item.ClassRoomId,
                    SubjectId = item.SubjectId
                }, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<TeacherTaskDto>> GetTasksByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default)
        {
            if (teacherId <= 0)
                throw new Exception("TeacherId düzgün deyil.");

            var teacher = await _unitOfWork.Teachers.GetByIdWithDetailsAsync(teacherId, cancellationToken);
            if (teacher == null)
                throw new Exception("Teacher tapılmadı.");

            return teacher.Tasks?
                .OrderBy(x => x.IsCompleted)
                .ThenBy(x => x.DueDate)
                .Select(x => new TeacherTaskDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    DueDate = AzerbaijanTimeHelper.ToBakuTime(x.DueDate),
                    Status = x.Status.ToString(),
                    IsCompleted = x.IsCompleted
                })
                .ToList() ?? new List<TeacherTaskDto>();
        }

        public async Task<TeacherOverviewStatsDto> GetOverviewStatsAsync(int teacherId, CancellationToken cancellationToken = default)
        {
            if (teacherId <= 0)
                throw new Exception("TeacherId düzgün deyil.");

            var teacher = await _unitOfWork.Teachers.GetByIdWithDetailsAsync(teacherId, cancellationToken);
            if (teacher == null)
                throw new Exception("Teacher tapılmadı.");

            return BuildOverviewStats(teacher);
        }

        private TeacherDto MapTeacherDto(Teacher teacher)
        {
            return new TeacherDto
            {
                Id = teacher.Id,
                UserId = teacher.UserId,
                FullName = teacher.FullName,
                Department = teacher.Department,
                UserName = teacher.User?.Username ?? string.Empty,
                Email = teacher.User?.Email ?? string.Empty,
                FirstName = teacher.User?.FirstName ?? string.Empty,
                LastName = teacher.User?.LastName ?? string.Empty,
                FatherName = teacher.User?.FatherName ?? string.Empty,
                PhoneNumber = teacher.User?.PhoneNumber,
                PhotoUrl = teacher.User?.PhotoUrl,
                Country = teacher.User?.Country,
                BirthDate = teacher.User?.BirthDate,
                Details = teacher.User?.Details,
                Specialization = teacher.Specialization,
                Status = teacher.Status.ToString(),
                IsActive = teacher.User?.IsActive ?? false
            };
        }

        private async Task<TeacherDetailsDto> BuildTeacherDetailsDtoAsync(Teacher teacher, CancellationToken cancellationToken)
        {
            var subjects = await GetSubjectsByTeacherIdAsync(teacher.Id, cancellationToken);
            var classRooms = await GetClassRoomsByTeacherIdAsync(teacher.Id, cancellationToken);

            return new TeacherDetailsDto
            {
                Id = teacher.Id,
                UserId = teacher.UserId,
                FullName = teacher.FullName,
                Department = teacher.Department,
                UserName = teacher.User?.Username ?? string.Empty,
                Email = teacher.User?.Email ?? string.Empty,
                Subjects = subjects,
                ClassRooms = classRooms,
                ExamCount = teacher.CreatedExams?.Count ?? 0,
                FirstName = teacher.User?.FirstName ?? string.Empty,
                LastName = teacher.User?.LastName ?? string.Empty,
                FatherName = teacher.User?.FatherName ?? string.Empty,
                PhoneNumber = teacher.User?.PhoneNumber,
                PhotoUrl = teacher.User?.PhotoUrl,
                Country = teacher.User?.Country,
                BirthDate = teacher.User?.BirthDate,
                Details = teacher.User?.Details,
                Specialization = teacher.Specialization,
                Status = teacher.Status.ToString(),
                IsActive = teacher.User?.IsActive ?? false,
                Tasks = teacher.Tasks?
                    .OrderBy(x => x.IsCompleted)
                    .ThenBy(x => x.DueDate)
                    .Select(x => new TeacherTaskDto
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        DueDate = x.DueDate,
                        Status = x.Status.ToString(),
                        IsCompleted = x.IsCompleted
                    })
                    .ToList() ?? new List<TeacherTaskDto>(),
                OverviewStats = BuildOverviewStats(teacher)
            };
        }

        private TeacherOverviewStatsDto BuildOverviewStats(Teacher teacher)
        {
            var activeAssignments = teacher.ClassTeacherSubjects?
                .ToList() ?? new List<ClassTeacherSubject>();

            var classRoomCount = activeAssignments
                .Select(x => x.ClassRoomId)
                .Distinct()
                .Count();

            var studentCount = activeAssignments
                .Where(x => x.ClassRoom != null)
                .SelectMany(x => x.ClassRoom.StudentClasses)
                .Select(x => x.StudentId)
                .Distinct()
                .Count();

            var pendingTaskCount = teacher.Tasks?.Count(x => !x.IsCompleted) ?? 0;
            var completedTaskCount = teacher.Tasks?.Count(x => x.IsCompleted) ?? 0;

            return new TeacherOverviewStatsDto
            {
                SubjectCount = teacher.TeacherSubjects?.Count(x => x.IsActive) ?? 0,
                ClassRoomCount = classRoomCount,
                StudentCount = studentCount,
                ExamCount = teacher.CreatedExams?.Count ?? 0,
                PendingTaskCount = pendingTaskCount,
                CompletedTaskCount = completedTaskCount
            };
        }
    }
}