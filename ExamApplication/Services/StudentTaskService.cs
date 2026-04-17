using ExamApplication.DTO.Notification;
using ExamApplication.DTO.Student;
using ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Services;
using ExamDomain.Entities;
using ExamDomain.Enum;

namespace ExamApplication.Services
{
    // YENI - SAGIRD UCUN
    public class StudentTaskService : IStudentTaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService; // YENI
        public StudentTaskService(
     IUnitOfWork unitOfWork,
     ICurrentUserService currentUserService,
     INotificationService notificationService) // YENI
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _notificationService = notificationService; // YENI
        }

        // YENI - SAGIRD UCUN
        public async Task<List<StudentTaskListItemDto>> GetMyTasksAsync(int? subjectId, CancellationToken cancellationToken = default)
        {
            var tasks = await LoadMyVisibleTasksAsync(subjectId, cancellationToken);

            return tasks
                .Select(MapListItem)
                .OrderByDescending(x => x.AssignedDate)
                .ToList();
        }

        // YENI - SAGIRD UCUN
        public async Task<StudentTaskDetailDto> GetMyTaskDetailAsync(int studentTaskId, CancellationToken cancellationToken = default)
        {
            var student = await GetCurrentStudentAsync(cancellationToken);

            var task = await _unitOfWork.StudentTasks
                .GetStudentTaskDetailAsync(studentTaskId, student.Id, cancellationToken);

            if (task == null)
                throw new KeyNotFoundException("Task tapılmadı.");

            return MapDetail(task);
        }

        // YENI - SAGIRD UCUN
        public async Task<StudentTaskDetailDto> SubmitMyTaskAsync(int studentTaskId, SubmitStudentTaskDto request, CancellationToken cancellationToken = default)
        {
            var student = await GetCurrentStudentAsync(cancellationToken);

            var task = await _unitOfWork.StudentTasks
                .GetStudentTaskDetailAsync(studentTaskId, student.Id, cancellationToken);

            if (task == null)
                throw new KeyNotFoundException("Task tapılmadı.");

            if (!task.IsActive)
                throw new InvalidOperationException("Bu task artıq aktiv deyil.");

            if (task.Status == StudentTaskStatus.Reviewed)
                throw new InvalidOperationException("Yoxlanılmış task yenidən submit edilə bilməz.");

            if (DateTime.UtcNow < task.AssignedDate)
                throw new InvalidOperationException("Task hələ aktiv deyil.");

            var submissionText = request.SubmissionText?.Trim();
            var submissionLink = request.SubmissionLink?.Trim();
            var submissionFileUrl = request.SubmissionFileUrl?.Trim();

            if (string.IsNullOrWhiteSpace(submissionText) &&
                string.IsNullOrWhiteSpace(submissionLink) &&
                string.IsNullOrWhiteSpace(submissionFileUrl))
            {
                throw new ArgumentException("Ən azı bir submit məlumatı daxil edilməlidir.");
            }

            task.SubmissionText = submissionText;
            task.SubmissionLink = submissionLink;
            task.SubmissionFileUrl = submissionFileUrl;
            task.SubmittedAt = DateTime.UtcNow;

            if (task.SubmittedAt.Value > task.DueDate)
            {
                task.Status = StudentTaskStatus.Late;
            }
            else
            {
                task.Status = StudentTaskStatus.Submitted;
            }

            _unitOfWork.StudentTasks.Update(task);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            // YENI - notify teacher about submitted task
            if (task.TeacherId.HasValue)
            {
                var teacher = await _unitOfWork.Teachers.GetByIdWithDetailsAsync(task.TeacherId.Value, cancellationToken);

                if (teacher?.UserId > 0)
                {
                    await _notificationService.CreateAsync(new CreateNotificationDto
                    {
                        UserId = teacher.UserId,
                        Title = "Yeni tapşırıq cavabı göndərildi",
                        Message = $"{student.FullName} tərəfindən {task.Title} tapşırığı üçün cavab göndərildi.",
                        Type = (int)NotificationType.Task,
                        Category = (int)NotificationCategory.TaskSubmitted,
                        Priority = (int)NotificationPriority.Medium,
                        RelatedEntityType = "StudentTask",
                        RelatedEntityId = task.Id,
                        ActionUrl = $"/teacher/tasks/{task.TaskGroupKey}",
                        Icon = "task-submitted",
                        MetadataJson = $@"{{""studentTaskId"":{task.Id},""studentId"":{student.Id},""taskGroupKey"":""{task.TaskGroupKey}""}}",
                        ExpiresAt = DateTime.UtcNow.AddDays(30)
                    }, cancellationToken);
                }
            }
            var updatedTask = await _unitOfWork.StudentTasks
                .GetStudentTaskDetailAsync(task.Id, student.Id, cancellationToken);

            return MapDetail(updatedTask!);
        }

        // YENI - SAGIRD UCUN
        public async Task<StudentTaskSummaryDto> GetMyTaskSummaryAsync(CancellationToken cancellationToken = default)
        {
            var tasks = await LoadMyVisibleTasksAsync(null, cancellationToken);

            var normalizedStatuses = tasks
                .Select(GetEffectiveStatus)
                .ToList();

            return new StudentTaskSummaryDto
            {
                TotalCount = tasks.Count,
                PendingCount = normalizedStatuses.Count(x => x == StudentTaskStatus.Pending),
                SubmittedCount = normalizedStatuses.Count(x => x == StudentTaskStatus.Submitted),
                ReviewedCount = normalizedStatuses.Count(x => x == StudentTaskStatus.Reviewed),
                LateCount = normalizedStatuses.Count(x => x == StudentTaskStatus.Late),
                MissingCount = normalizedStatuses.Count(x => x == StudentTaskStatus.Missing)
            };
        }

        // YENI - SAGIRD UCUN
        private async Task<List<StudentTask>> LoadMyVisibleTasksAsync(int? subjectId, CancellationToken cancellationToken)
        {
            var student = await GetCurrentStudentAsync(cancellationToken);

            var tasks = await _unitOfWork.StudentTasks
                .GetByStudentIdAsync(student.Id, cancellationToken);

            var query = tasks
                .Where(x => x.IsActive);

            if (subjectId.HasValue)
            {
                query = query.Where(x => x.SubjectId == subjectId.Value);
            }

            return query
                .OrderByDescending(x => x.AssignedDate)
                .ToList();
        }

        // YENI - SAGIRD UCUN
        private async Task<Student> GetCurrentStudentAsync(CancellationToken cancellationToken)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            if (currentUser?.UserId == null)
                throw new UnauthorizedAccessException("Cari istifadəçi tapılmadı.");

            var student = await _unitOfWork.Students.GetByUserIdAsync(currentUser.UserId, cancellationToken);

            if (student == null)
                throw new KeyNotFoundException("Şagird profili tapılmadı.");

            return student;
        }

        // YENI - SAGIRD UCUN
        private StudentTaskListItemDto MapListItem(StudentTask task)
        {
            var effectiveStatus = GetEffectiveStatus(task);

            return new StudentTaskListItemDto
            {
                Id = task.Id,
                TaskGroupKey = task.TaskGroupKey,
                Title = task.Title,
                Description = task.Description,
                SubjectId = task.SubjectId,
                SubjectName = task.Subject?.Name ?? string.Empty,
                TeacherId = task.TeacherId,
                TeacherName = task.Teacher?.FullName
                              ?? ((task.Teacher?.User?.FirstName ?? "") + " " + (task.Teacher?.User?.LastName ?? "")).Trim(),
                ClassRoomId = task.ClassRoomId,
                ClassName = task.ClassRoom?.Name ?? string.Empty,
                AssignedDate = task.AssignedDate,
                DueDate = task.DueDate,
                Status = effectiveStatus.ToString(),
                IsLate = effectiveStatus == StudentTaskStatus.Late,
                CanSubmit = CanSubmit(task, effectiveStatus),
                IsSubmitted = task.SubmittedAt.HasValue,
                IsReviewed = effectiveStatus == StudentTaskStatus.Reviewed,
                Score = task.Score,
                MaxScore = task.MaxScore,
                SubmittedAt = task.SubmittedAt
            };
        }

        // YENI - SAGIRD UCUN
        private StudentTaskDetailDto MapDetail(StudentTask task)
        {
            var effectiveStatus = GetEffectiveStatus(task);

            return new StudentTaskDetailDto
            {
                Id = task.Id,
                TaskGroupKey = task.TaskGroupKey,
                Title = task.Title,
                Description = task.Description,
                SubjectId = task.SubjectId,
                SubjectName = task.Subject?.Name ?? string.Empty,
                TeacherId = task.TeacherId,
                TeacherName = task.Teacher?.FullName
                              ?? ((task.Teacher?.User?.FirstName ?? "") + " " + (task.Teacher?.User?.LastName ?? "")).Trim(),
                ClassRoomId = task.ClassRoomId,
                ClassName = task.ClassRoom?.Name ?? string.Empty,
                AssignedDate = task.AssignedDate,
                DueDate = task.DueDate,
                Status = effectiveStatus.ToString(),
                IsLate = effectiveStatus == StudentTaskStatus.Late,
                CanSubmit = CanSubmit(task, effectiveStatus),
                IsSubmitted = task.SubmittedAt.HasValue,
                IsReviewed = effectiveStatus == StudentTaskStatus.Reviewed,
                Score = task.Score,
                MaxScore = task.MaxScore,
                Link = task.Link,
                Note = task.Note,
                SubmissionText = task.SubmissionText,
                SubmissionLink = task.SubmissionLink,
                SubmissionFileUrl = task.SubmissionFileUrl,
                SubmittedAt = task.SubmittedAt,
                Feedback = task.Feedback,
                CheckedAt = task.CheckedAt
            };
        }

        // YENI - SAGIRD UCUN
        private StudentTaskStatus GetEffectiveStatus(StudentTask task)
        {
            if (task.Status == StudentTaskStatus.Reviewed)
                return StudentTaskStatus.Reviewed;

            if (!task.SubmittedAt.HasValue && DateTime.UtcNow > task.DueDate)
                return StudentTaskStatus.Missing;

            if (task.SubmittedAt.HasValue && task.SubmittedAt.Value > task.DueDate)
                return StudentTaskStatus.Late;

            if (task.SubmittedAt.HasValue)
                return StudentTaskStatus.Submitted;

            return StudentTaskStatus.Pending;
        }

        // YENI - SAGIRD UCUN
        private bool CanSubmit(StudentTask task, StudentTaskStatus effectiveStatus)
        {
            if (!task.IsActive)
                return false;

            if (effectiveStatus == StudentTaskStatus.Reviewed)
                return false;

            if (DateTime.UtcNow < task.AssignedDate)
                return false;

            return true;
        }
    }
}