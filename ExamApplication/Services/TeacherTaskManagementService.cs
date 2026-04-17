using ExamApplication.DTO.Notification;
using ExamApplication.DTO.Teacher.Task;
using ExamApplication.DTO.Teacher.Task.ExamApplication.DTO.Teacher.Tasks;
using ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Services;
using ExamDomain.Entities;
using ExamDomain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Services
{
    public class TeacherTaskManagementService : ITeacherTaskManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService; // YENI
        public TeacherTaskManagementService(
             IUnitOfWork unitOfWork,
             ICurrentUserService currentUserService,
             INotificationService notificationService) // YENI
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _notificationService = notificationService; // YENI
        }

        public async Task<List<TeacherTaskClassSummaryDto>> GetMyTaskClassesAsync(CancellationToken cancellationToken = default)
        {
            var teacher = await GetCurrentTeacherAsync(cancellationToken);

            var classRelations = teacher.ClassTeacherSubjects
                .Where(x => x.ClassRoom != null && x.Subject != null)
                .GroupBy(x => x.ClassRoomId)
                .ToList();

            var teacherTasks = await _unitOfWork.StudentTasks.GetByTeacherIdAsync(teacher.Id, cancellationToken);

            var result = new List<TeacherTaskClassSummaryDto>();

            foreach (var group in classRelations)
            {
                var first = group.First();
                var classRoom = first.ClassRoom!;
                var classStudentCount = classRoom.StudentClasses?.Count ?? 0;

                var classTasks = teacherTasks
                    .Where(x => x.ClassRoomId == classRoom.Id)
                    .GroupBy(x => x.TaskGroupKey)
                    .Select(x => x.ToList())
                    .ToList();

                result.Add(new TeacherTaskClassSummaryDto
                {
                    ClassRoomId = classRoom.Id,
                    ClassName = classRoom.Name,
                    AcademicYear = classRoom.AcademicYear,
                    Room = classRoom.Room,
                    StudentCount = classStudentCount,
                    Subjects = group
                        .Where(x => x.Subject != null)
                        .GroupBy(x => x.SubjectId)
                        .Select(x => new TeacherTaskClassSubjectDto
                        {
                            SubjectId = x.First().SubjectId,
                            SubjectName = x.First().Subject!.Name
                        })
                        .OrderBy(x => x.SubjectName)
                        .ToList(),
                    TotalTaskCount = classTasks.Count,
                    ActiveTaskCount = classTasks.Count(x => GetTaskState(x.First().AssignedDate, x.First().DueDate) == "Davam edir"),
                    CompletedTaskCount = classTasks.Count(x => GetTaskState(x.First().AssignedDate, x.First().DueDate) == "Bitib"),
                    PendingReviewCount = classTasks.Count(x =>
                        GetTaskState(x.First().AssignedDate, x.First().DueDate) == "Bitib" &&
                        x.Any(t => t.Status != StudentTaskStatus.Reviewed))
                });
            }

            return result
                .OrderBy(x => x.ClassName)
                .ToList();
        }

        public async Task<List<TeacherClassTaskListItemDto>> GetClassTasksAsync(int classRoomId, CancellationToken cancellationToken = default)
        {
            var teacher = await GetCurrentTeacherAsync(cancellationToken);

            var tasks = await _unitOfWork.StudentTasks.GetByTeacherAndClassRoomIdAsync(teacher.Id, classRoomId, cancellationToken);

            return tasks
                .GroupBy(x => x.TaskGroupKey)
                .Select(group =>
                {
                    var first = group.First();
                    return new TeacherClassTaskListItemDto
                    {
                        TaskGroupKey = first.TaskGroupKey,
                        ClassRoomId = first.ClassRoomId ?? 0,
                        ClassName = first.ClassRoom?.Name ?? string.Empty,
                        SubjectId = first.SubjectId,
                        SubjectName = first.Subject?.Name ?? string.Empty,
                        TeacherId = first.TeacherId,
                        TeacherName = first.Teacher?.FullName ?? string.Empty,
                        Title = first.Title,
                        Description = first.Description,
                        AssignedDate = first.AssignedDate,
                        DueDate = first.DueDate,
                        MaxScore = first.MaxScore,
                        TaskState = GetTaskState(first.AssignedDate, first.DueDate),
                        TotalStudentCount = group.Count(),
                        SubmittedCount = group.Count(x => x.Status == StudentTaskStatus.Submitted || x.Status == StudentTaskStatus.Late || x.Status == StudentTaskStatus.Reviewed),
                        MissingCount = group.Count(x => x.Status == StudentTaskStatus.Missing),
                        ReviewedCount = group.Count(x => x.Status == StudentTaskStatus.Reviewed)
                    };
                })
                .OrderByDescending(x => x.AssignedDate)
                .ToList();
        }

        public async Task<TeacherTaskDetailDto> GetTaskDetailAsync(string taskGroupKey, CancellationToken cancellationToken = default)
        {
            var teacher = await GetCurrentTeacherAsync(cancellationToken);

            var tasks = await _unitOfWork.StudentTasks.GetByTaskGroupKeyAsync(taskGroupKey, cancellationToken);

            if (!tasks.Any())
                throw new KeyNotFoundException("Task tapılmadı.");

            if (tasks.Any(x => x.TeacherId != teacher.Id))
                throw new UnauthorizedAccessException("Bu task sizə aid deyil.");

            return MapTaskDetail(tasks);
        }

        public async Task<TeacherTaskDetailDto> CreateClassTaskAsync(CreateTeacherClassTaskDto request, CancellationToken cancellationToken = default)
        {
            var teacher = await GetCurrentTeacherAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Task başlığı boş ola bilməz.");

            if (request.DueDate <= request.AssignedDate)
                throw new ArgumentException("Son tarix başlanğıc tarixindən böyük olmalıdır.");

            if (request.MaxScore <= 0)
                throw new ArgumentException("Maksimum bal 0-dan böyük olmalıdır.");

            var classRoom = await _unitOfWork.ClassRooms.GetByIdWithDetailsAsync(request.ClassRoomId, cancellationToken);
            if (classRoom == null)
                throw new KeyNotFoundException("Sinif tapılmadı.");

            var subject = await _unitOfWork.Subjects.GetByIdAsync(request.SubjectId, cancellationToken);
            if (subject == null)
                throw new KeyNotFoundException("Fənn tapılmadı.");

            var relationExists = teacher.ClassTeacherSubjects.Any(x =>
                x.ClassRoomId == request.ClassRoomId &&
                x.SubjectId == request.SubjectId &&
                x.IsActive);

            if (!relationExists)
                throw new InvalidOperationException("Bu sinif və fənn müəllimə aid deyil.");

            // YENI
            // ClassRoom navigation-a guvenmek yerine StudentClasses cedvelinden birbasa aktiv sagirdleri gotur
            var activeStudentClasses = await _unitOfWork.StudentClasses
                .GetActiveByClassRoomIdAsync(request.ClassRoomId, cancellationToken);

            var studentIds = activeStudentClasses
                .Where(x => x.StudentId > 0)
                .Select(x => x.StudentId)
                .Distinct()
                .ToList();

            if (!studentIds.Any())
                throw new InvalidOperationException("Seçilmiş sinifdə aktiv şagird tapılmadı.");

            var taskGroupKey = Guid.NewGuid().ToString("N");

            var studentTasks = studentIds.Select(studentId => new StudentTask
            {
                StudentId = studentId,
                TeacherId = teacher.Id,
                SubjectId = request.SubjectId,
                ClassRoomId = request.ClassRoomId,
                TaskGroupKey = taskGroupKey,
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                AssignedDate = request.AssignedDate,
                DueDate = request.DueDate,
                Status = StudentTaskStatus.Pending,
                Score = 0,
                MaxScore = request.MaxScore,
                Link = request.Link?.Trim(),
                Note = request.Note?.Trim(),
                IsActive = true
            }).ToList();

            await _unitOfWork.StudentTasks.AddRangeAsync(studentTasks, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            // YENI - task assigned notifications
            var students = await _unitOfWork.Students.GetByIdsWithDetailsAsync(studentIds, cancellationToken);

            var taskNotifications = students
                .Where(x => x.UserId > 0)
                .Select(student => new CreateNotificationDto
                {
                    UserId = student.UserId,
                    Title = "Yeni tapşırıq əlavə edildi",
                    Message = $"{request.Title.Trim()} tapşırığı sizə təyin edildi.",
                    Type = (int)NotificationType.Task,
                    Category = (int)NotificationCategory.TaskAssigned,
                    Priority = (int)NotificationPriority.Medium,
                    RelatedEntityType = "StudentTaskGroup",
                    RelatedEntityId = null,
                    ActionUrl = "/student/tasks",
                    Icon = "task",
                    MetadataJson = $@"{{""taskGroupKey"":""{taskGroupKey}"",""classRoomId"":{request.ClassRoomId},""subjectId"":{request.SubjectId}}}",
                    ExpiresAt = request.DueDate.AddDays(7)
                })
                .ToList();

            await _notificationService.CreateBulkAsync(taskNotifications, cancellationToken);
            var created = await _unitOfWork.StudentTasks.GetByTaskGroupKeyAsync(taskGroupKey, cancellationToken);
            return MapTaskDetail(created);
        }

        public async Task<TeacherTaskDetailDto> UpdateClassTaskAsync(UpdateTeacherClassTaskDto request, CancellationToken cancellationToken = default)
        {
            var teacher = await GetCurrentTeacherAsync(cancellationToken);

            var tasks = await _unitOfWork.StudentTasks.GetByTaskGroupKeyAsync(request.TaskGroupKey, cancellationToken);

            if (!tasks.Any())
                throw new KeyNotFoundException("Task tapılmadı.");

            if (tasks.Any(x => x.TeacherId != teacher.Id))
                throw new UnauthorizedAccessException("Bu task sizə aid deyil.");

            var first = tasks.First();

            if (DateTime.UtcNow > first.DueDate)
                throw new InvalidOperationException("Vaxtı bitmiş task redaktə edilə bilməz.");

            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Task başlığı boş ola bilməz.");

            if (request.DueDate <= request.AssignedDate)
                throw new ArgumentException("Son tarix başlanğıc tarixindən böyük olmalıdır.");

            if (request.MaxScore <= 0)
                throw new ArgumentException("Maksimum bal 0-dan böyük olmalıdır.");

            foreach (var task in tasks)
            {
                task.Title = request.Title.Trim();
                task.Description = request.Description?.Trim();
                task.AssignedDate = request.AssignedDate;
                task.DueDate = request.DueDate;
                task.MaxScore = request.MaxScore;
                task.Link = request.Link?.Trim();
                task.Note = request.Note?.Trim();
            }

            _unitOfWork.StudentTasks.UpdateRange(tasks);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updated = await _unitOfWork.StudentTasks.GetByTaskGroupKeyAsync(request.TaskGroupKey, cancellationToken);
            return MapTaskDetail(updated);
        }

        public async Task DeleteClassTaskAsync(string taskGroupKey, CancellationToken cancellationToken = default)
        {
            var teacher = await GetCurrentTeacherAsync(cancellationToken);

            var tasks = await _unitOfWork.StudentTasks.GetByTaskGroupKeyAsync(taskGroupKey, cancellationToken);

            if (!tasks.Any())
                throw new KeyNotFoundException("Task tapılmadı.");

            if (tasks.Any(x => x.TeacherId != teacher.Id))
                throw new UnauthorizedAccessException("Bu task sizə aid deyil.");

            _unitOfWork.StudentTasks.RemoveRange(tasks);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<StudentTaskSubmissionDetailDto> GetStudentTaskSubmissionDetailAsync(string taskGroupKey, int studentTaskId, CancellationToken cancellationToken = default)
        {
            var teacher = await GetCurrentTeacherAsync(cancellationToken);

            var task = await _unitOfWork.StudentTasks.GetByTaskGroupKeyAndStudentTaskIdAsync(taskGroupKey, studentTaskId, cancellationToken);

            if (task == null)
                throw new KeyNotFoundException("Şagird task nəticəsi tapılmadı.");

            if (task.TeacherId != teacher.Id)
                throw new UnauthorizedAccessException("Bu nəticə sizə aid deyil.");

            return MapSubmissionDetail(task);
        }

        public async Task<StudentTaskSubmissionDetailDto> GradeStudentTaskAsync(GradeStudentTaskDto request, CancellationToken cancellationToken = default)
        {
            var teacher = await GetCurrentTeacherAsync(cancellationToken);

            var task = await _unitOfWork.StudentTasks.GetByIdAsync(request.StudentTaskId, cancellationToken);

            if (task == null)
                throw new KeyNotFoundException("Task nəticəsi tapılmadı.");

            if (task.TeacherId != teacher.Id)
                throw new UnauthorizedAccessException("Bu nəticə sizə aid deyil.");

            if (DateTime.UtcNow <= task.DueDate)
                throw new InvalidOperationException("Taskın vaxtı bitmədən qiymətləndirmə edilə bilməz.");

            if (request.Score < 0 || request.Score > task.MaxScore)
                throw new ArgumentException($"Bal 0 ilə {task.MaxScore} arasında olmalıdır.");

            task.Score = request.Score;
            task.Feedback = request.Feedback?.Trim();
            task.CheckedAt = DateTime.UtcNow;
            task.Status = StudentTaskStatus.Reviewed;

            _unitOfWork.StudentTasks.Update(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            // YENI - task reviewed notification
            var student = await _unitOfWork.Students.GetByIdWithDetailsAsync(task.StudentId, cancellationToken);

            if (student?.UserId > 0)
            {
                await _notificationService.CreateAsync(new CreateNotificationDto
                {
                    UserId = student.UserId,
                    Title = "Tapşırığınız yoxlanıldı",
                    Message = $"{task.Title} tapşırığınız müəllim tərəfindən qiymətləndirildi.",
                    Type = (int)NotificationType.Task,
                    Category = (int)NotificationCategory.TaskReviewed,
                    Priority = (int)NotificationPriority.High,
                    RelatedEntityType = "StudentTask",
                    RelatedEntityId = task.Id,
                    ActionUrl = $"/student/tasks/{task.Id}",
                    Icon = "task-reviewed",
                    MetadataJson = $@"{{""studentTaskId"":{task.Id},""score"":{task.Score},""maxScore"":{task.MaxScore}}}",
                    ExpiresAt = DateTime.UtcNow.AddDays(30)
                }, cancellationToken);
            }
            var updated = await _unitOfWork.StudentTasks.GetByIdAsync(task.Id, cancellationToken);
            return MapSubmissionDetail(updated!);
        }

        private async Task<ExamDomain.Entities.Teacher> GetCurrentTeacherAsync(CancellationToken cancellationToken)
        {
            var currentUser = _currentUserService.GetCurrentUser();
            if (currentUser?.UserId == null)
                throw new UnauthorizedAccessException("Cari istifadəçi tapılmadı.");

            var teacher = await _unitOfWork.Teachers.GetByUserIdWithDetailsAsync(currentUser.UserId, cancellationToken);
            if (teacher == null)
                throw new KeyNotFoundException("Müəllim profili tapılmadı.");

            return teacher;
        }

        private TeacherTaskDetailDto MapTaskDetail(List<StudentTask> tasks)
        {
            var first = tasks.First();

            return new TeacherTaskDetailDto
            {
                TaskGroupKey = first.TaskGroupKey,
                ClassRoomId = first.ClassRoomId ?? 0,
                ClassName = first.ClassRoom?.Name ?? string.Empty,
                SubjectId = first.SubjectId,
                SubjectName = first.Subject?.Name ?? string.Empty,
                TeacherId = first.TeacherId,
                TeacherName = first.Teacher?.FullName ?? string.Empty,
                Title = first.Title,
                Description = first.Description,
                Link = first.Link,
                Note = first.Note,
                AssignedDate = first.AssignedDate,
                DueDate = first.DueDate,
                MaxScore = first.MaxScore,
                TaskState = GetTaskState(first.AssignedDate, first.DueDate),
                TotalStudentCount = tasks.Count,
                SubmittedCount = tasks.Count(x => x.Status == StudentTaskStatus.Submitted || x.Status == StudentTaskStatus.Late || x.Status == StudentTaskStatus.Reviewed),
                MissingCount = tasks.Count(x => x.Status == StudentTaskStatus.Missing),
                ReviewedCount = tasks.Count(x => x.Status == StudentTaskStatus.Reviewed),
                Students = tasks
                    .OrderBy(x => x.Student.FullName)
                    .Select(MapSubmissionListItem)
                    .ToList()
            };
        }

        private StudentTaskSubmissionListItemDto MapSubmissionListItem(StudentTask task)
        {
            return new StudentTaskSubmissionListItemDto
            {
                StudentTaskId = task.Id,
                StudentId = task.StudentId,
                FullName = task.Student?.FullName ?? string.Empty,
                StudentNumber = task.Student?.StudentNumber ?? string.Empty,
                PhotoUrl = task.Student?.User?.PhotoUrl,
                SubmissionStatus = MapSubmissionStatus(task),
                SubmittedAt = task.SubmittedAt,
                Score = task.Score,
                MaxScore = task.MaxScore,
                IsReviewed = task.Status == StudentTaskStatus.Reviewed
            };
        }

        private StudentTaskSubmissionDetailDto MapSubmissionDetail(StudentTask task)
        {
            return new StudentTaskSubmissionDetailDto
            {
                StudentTaskId = task.Id,
                StudentId = task.StudentId,
                FullName = task.Student?.FullName ?? string.Empty,
                StudentNumber = task.Student?.StudentNumber ?? string.Empty,
                PhotoUrl = task.Student?.User?.PhotoUrl,
                Title = task.Title,
                TaskDescription = task.Description,
                TaskLink = task.Link,
                TaskNote = task.Note,
                AssignedDate = task.AssignedDate,
                DueDate = task.DueDate,
                SubmissionStatus = MapSubmissionStatus(task),
                SubmissionText = task.SubmissionText,
                SubmissionLink = task.SubmissionLink,
                SubmissionFileUrl = task.SubmissionFileUrl,
                SubmittedAt = task.SubmittedAt,
                Score = task.Score,
                MaxScore = task.MaxScore,
                Feedback = task.Feedback,
                CheckedAt = task.CheckedAt
            };
        }

        private static string GetTaskState(DateTime assignedDate, DateTime dueDate)
        {
            var now = DateTime.UtcNow;

            if (now < assignedDate)
                return "Gözləyir";

            if (now <= dueDate)
                return "Davam edir";

            return "Bitib";
        }

        private static string MapSubmissionStatus(StudentTask task)
        {
            return task.Status switch
            {
                StudentTaskStatus.Pending => DateTime.UtcNow > task.DueDate ? "Təhvil verməyib" : "Gözləyir",
                StudentTaskStatus.Submitted => "Vaxtında təhvil verib",
                StudentTaskStatus.Late => "Gec təhvil verib",
                StudentTaskStatus.Reviewed => "Yoxlanıb",
                StudentTaskStatus.Missing => "Təhvil verməyib",
                _ => "Naməlum"
            };
        }
    }
}
