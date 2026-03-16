using ExamDomain.Entities;
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

        Task<List<Notification>> GetUnreadByUserIdAsync(int userId, CancellationToken cancellationToken = default);

        Task<int> GetUnreadCountByUserIdAsync(int userId, CancellationToken cancellationToken = default);

        Task AddAsync(Notification notification, CancellationToken cancellationToken = default);

        void Update(Notification notification);

        void Remove(Notification notification);
    }
}
