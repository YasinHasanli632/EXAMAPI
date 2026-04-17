using ExamDomain.Entities;
using ExamDomain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface INotificationRepository
    {
        Task<Notification?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<List<Notification>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

        Task<List<Notification>> GetByUserIdAsync(
            int userId,
            bool? isRead,
            int? type = null,
            CancellationToken cancellationToken = default);

        Task<List<Notification>> GetUnreadByUserIdAsync(int userId, CancellationToken cancellationToken = default);

        Task<int> GetUnreadCountByUserIdAsync(int userId, CancellationToken cancellationToken = default);

        Task<List<Notification>> GetLatestByUserIdAsync(
            int userId,
            int take,
            CancellationToken cancellationToken = default);

        Task AddAsync(Notification notification, CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<Notification> notifications, CancellationToken cancellationToken = default);

        void Update(Notification notification);

        void Remove(Notification notification);
    }
}
