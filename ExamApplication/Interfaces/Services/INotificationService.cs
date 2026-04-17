using ExamApplication.DTO.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    public interface INotificationService
    {
        Task<List<NotificationListItemDto>> GetMyNotificationsAsync(
            bool? isRead = null,
            int? type = null,
            CancellationToken cancellationToken = default);

        Task<List<NotificationListItemDto>> GetMyLatestNotificationsAsync(
            int take = 5,
            CancellationToken cancellationToken = default);

        Task<NotificationUnreadCountDto> GetMyUnreadCountAsync(
            CancellationToken cancellationToken = default);

        Task MarkAsReadAsync(
            int notificationId,
            CancellationToken cancellationToken = default);

        Task MarkAllAsReadAsync(
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            int notificationId,
            CancellationToken cancellationToken = default);

        Task CreateAsync(
            CreateNotificationDto request,
            CancellationToken cancellationToken = default);

        Task CreateBulkAsync(
            List<CreateNotificationDto> requests,
            CancellationToken cancellationToken = default);

        Task CreateExamPublishNotificationsAsync(
            int examId,
            CancellationToken cancellationToken = default);
    }

}
