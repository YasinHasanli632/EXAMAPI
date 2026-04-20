using ExamApplication.DTO.Notification;
using ExamApplication.Helper;
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
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public NotificationService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<List<NotificationListItemDto>> GetMyNotificationsAsync(
            bool? isRead = null,
            int? type = null,
            CancellationToken cancellationToken = default)
        {
            var currentUser = GetRequiredCurrentUser();

            if (type.HasValue && !Enum.IsDefined(typeof(NotificationType), type.Value))
            {
                throw new ArgumentException("Notification type düzgün deyil.");
            }

            var notifications = await _unitOfWork.Notifications.GetByUserIdAsync(
                currentUser.UserId,
                isRead,
                type,
                cancellationToken);

            return notifications
                .Select(MapToListItemDto)
                .ToList();
        }

        public async Task<List<NotificationListItemDto>> GetMyLatestNotificationsAsync(
            int take = 5,
            CancellationToken cancellationToken = default)
        {
            var currentUser = GetRequiredCurrentUser();

            if (take <= 0)
            {
                take = 5;
            }

            if (take > 50)
            {
                take = 50;
            }

            var notifications = await _unitOfWork.Notifications.GetLatestByUserIdAsync(
                currentUser.UserId,
                take,
                cancellationToken);

            return notifications
                .Select(MapToListItemDto)
                .ToList();
        }

        public async Task<NotificationUnreadCountDto> GetMyUnreadCountAsync(
            CancellationToken cancellationToken = default)
        {
            var currentUser = GetRequiredCurrentUser();

            var count = await _unitOfWork.Notifications.GetUnreadCountByUserIdAsync(
                currentUser.UserId,
                cancellationToken);

            return new NotificationUnreadCountDto
            {
                UnreadCount = count
            };
        }

        public async Task MarkAsReadAsync(
            int notificationId,
            CancellationToken cancellationToken = default)
        {
            var currentUser = GetRequiredCurrentUser();

            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId, cancellationToken);
            if (notification == null)
            {
                throw new KeyNotFoundException("Bildiriş tapılmadı.");
            }

            if (notification.UserId != currentUser.UserId)
            {
                throw new UnauthorizedAccessException("Bu bildiriş sizə aid deyil.");
            }

            if (notification.IsRead)
            {
                return;
            }

            notification.IsRead = true;

            // YENI
            notification.ReadAt = DateTime.UtcNow;

            notification.UpdatedAt = DateTime.UtcNow;
            notification.UpdatedByUserId = currentUser.UserId;

            _unitOfWork.Notifications.Update(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task MarkAllAsReadAsync(CancellationToken cancellationToken = default)
        {
            var currentUser = GetRequiredCurrentUser();

            var unreadNotifications = await _unitOfWork.Notifications.GetUnreadByUserIdAsync(
                currentUser.UserId,
                cancellationToken);

            if (!unreadNotifications.Any())
            {
                return;
            }

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;

                // YENI
                notification.ReadAt = DateTime.UtcNow;

                notification.UpdatedAt = DateTime.UtcNow;
                notification.UpdatedByUserId = currentUser.UserId;
                _unitOfWork.Notifications.Update(notification);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int notificationId, CancellationToken cancellationToken = default)
        {
            var currentUser = GetRequiredCurrentUser();

            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId, cancellationToken);
            if (notification == null)
            {
                throw new KeyNotFoundException("Bildiriş tapılmadı.");
            }

            if (notification.UserId != currentUser.UserId)
            {
                throw new UnauthorizedAccessException("Bu bildiriş sizə aid deyil.");
            }

            _unitOfWork.Notifications.Remove(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateAsync(CreateNotificationDto request, CancellationToken cancellationToken = default)
        {
            ValidateCreateRequest(request);

            // YENI
            var userExists = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (userExists == null)
            {
                throw new KeyNotFoundException("Notification üçün istifadəçi tapılmadı.");
            }

            var currentUser = _currentUserService.GetCurrentUser();

            var entity = new Notification
            {
                UserId = request.UserId,
                Title = request.Title.Trim(),
                Message = request.Message.Trim(),
                Type = (NotificationType)request.Type,

                // YENI
                Category = Enum.IsDefined(typeof(NotificationCategory), request.Category)
                    ? (NotificationCategory)request.Category
                    : NotificationCategory.SystemWarning,

                // YENI
                Priority = Enum.IsDefined(typeof(NotificationPriority), request.Priority)
                    ? (NotificationPriority)request.Priority
                    : NotificationPriority.Medium,

                IsRead = false,
                ReadAt = null,
                RelatedEntityType = NormalizeNullable(request.RelatedEntityType),
                RelatedEntityId = request.RelatedEntityId,
                ActionUrl = NormalizeNullable(request.ActionUrl),

                // YENI
                Icon = NormalizeNullable(request.Icon),
                MetadataJson = NormalizeNullable(request.MetadataJson),
                ExpiresAt = request.ExpiresAt,

                // YENI
                CreatedByUserId = currentUser?.UserId
            };

            await _unitOfWork.Notifications.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateBulkAsync(List<CreateNotificationDto> requests, CancellationToken cancellationToken = default)
        {
            if (requests == null || requests.Count == 0)
            {
                return;
            }

            foreach (var request in requests)
            {
                ValidateCreateRequest(request);
            }

            // YENI - userId-ləri toplu şəkildə yoxlayaq
            var userIds = requests
                .Select(x => x.UserId)
                .Distinct()
                .ToList();

            var users = await _unitOfWork.Users.GetByIdsAsync(userIds, cancellationToken);
            var validUserIds = users
                .Select(x => x.Id)
                .ToHashSet();

            var entities = new List<Notification>();
            var currentUser = _currentUserService.GetCurrentUser();

            foreach (var request in requests)
            {
                if (!validUserIds.Contains(request.UserId))
                {
                    continue;
                }

                entities.Add(new Notification
                {
                    UserId = request.UserId,
                    Title = request.Title.Trim(),
                    Message = request.Message.Trim(),
                    Type = (NotificationType)request.Type,

                    Category = Enum.IsDefined(typeof(NotificationCategory), request.Category)
                        ? (NotificationCategory)request.Category
                        : NotificationCategory.SystemWarning,

                    Priority = Enum.IsDefined(typeof(NotificationPriority), request.Priority)
                        ? (NotificationPriority)request.Priority
                        : NotificationPriority.Medium,

                    IsRead = false,
                    ReadAt = null,
                    RelatedEntityType = NormalizeNullable(request.RelatedEntityType),
                    RelatedEntityId = request.RelatedEntityId,
                    ActionUrl = NormalizeNullable(request.ActionUrl),
                    Icon = NormalizeNullable(request.Icon),
                    MetadataJson = NormalizeNullable(request.MetadataJson),
                    ExpiresAt = request.ExpiresAt,
                    CreatedByUserId = currentUser?.UserId
                });
            }

            if (!entities.Any())
            {
                return;
            }

            await _unitOfWork.Notifications.AddRangeAsync(entities, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateExamPublishNotificationsAsync(int examId, CancellationToken cancellationToken = default)
        {
            var exam = await _unitOfWork.Exams.GetByIdWithDetailsAsync(examId, cancellationToken);
            if (exam == null)
            {
                throw new KeyNotFoundException("İmtahan tapılmadı.");
            }

            if (!exam.ClassRoomId.HasValue)
            {
                return;
            }

            var studentClasses = await _unitOfWork.StudentClasses.GetActiveByClassRoomIdAsync(
                exam.ClassRoomId.Value,
                cancellationToken);

            var studentIds = studentClasses
                .Select(x => x.StudentId)
                .Distinct()
                .ToList();

            if (!studentIds.Any())
            {
                return;
            }

            var students = await _unitOfWork.Students.GetByIdsWithDetailsAsync(studentIds, cancellationToken);

            var notifications = students
                .Where(x => x.UserId > 0)
                .Select(student => new CreateNotificationDto
                {
                    UserId = student.UserId,
                    Title = "Yeni imtahan yayımlandı",
                    Message = $"{exam.Title} imtahanı yayımlandı və sizin üçün aktivləşdirildi.",
                    Type = (int)NotificationType.Exam,

                    // YENI
                    Category = (int)NotificationCategory.ExamPublished,
                    Priority = (int)NotificationPriority.High,

                    RelatedEntityType = "Exam",
                    RelatedEntityId = exam.Id,
                    ActionUrl = $"/student/exams/{exam.Id}",

                    // YENI
                    Icon = "exam",
                    MetadataJson = $@"{{""examId"":{exam.Id},""classRoomId"":{exam.ClassRoomId}}}",
                    ExpiresAt = exam.EndTime.AddDays(7)
                })
                .ToList();

            await CreateBulkAsync(notifications, cancellationToken);
        }

        private NotificationListItemDto MapToListItemDto(Notification notification)
        {
            return new NotificationListItemDto
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type.ToString(),

                // YENI
                Category = notification.Category.ToString(),
                Priority = notification.Priority.ToString(),

                IsRead = notification.IsRead,
                CreatedAt = AzerbaijanTimeHelper.ToBakuTime(notification.CreatedAt),
                ReadAt = AzerbaijanTimeHelper.ToBakuTime(notification.ReadAt),

                RelatedEntityType = notification.RelatedEntityType,
                RelatedEntityId = notification.RelatedEntityId,
                ActionUrl = notification.ActionUrl,

                // YENI
                Icon = notification.Icon
            };
        }

        private void ValidateCreateRequest(CreateNotificationDto request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.UserId <= 0)
            {
                throw new ArgumentException("UserId düzgün deyil.");
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ArgumentException("Notification title boş ola bilməz.");
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                throw new ArgumentException("Notification message boş ola bilməz.");
            }

            if (!Enum.IsDefined(typeof(NotificationType), request.Type))
            {
                throw new ArgumentException("Notification type düzgün deyil.");
            }

            // YENI
            if (request.Category > 0 && !Enum.IsDefined(typeof(NotificationCategory), request.Category))
            {
                throw new ArgumentException("Notification category düzgün deyil.");
            }

            // YENI
            if (request.Priority > 0 && !Enum.IsDefined(typeof(NotificationPriority), request.Priority))
            {
                throw new ArgumentException("Notification priority düzgün deyil.");
            }
        }

        private string? NormalizeNullable(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private ExamDomain.ValueObjects.JwtUserInfo GetRequiredCurrentUser()
        {
            var currentUser = _currentUserService.GetCurrentUser();
            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("Cari istifadəçi tapılmadı.");
            }

            return currentUser;
        }
    }
}
