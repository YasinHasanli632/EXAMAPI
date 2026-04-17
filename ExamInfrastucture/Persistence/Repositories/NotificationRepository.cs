using ExamApplication.Interfaces.Repository;
using ExamDomain.Entities;
using ExamDomain.Enum;
using ExamInfrastucture.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamInfrastucture.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Notification?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<Notification>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Where(x => !x.ExpiresAt.HasValue || x.ExpiresAt > DateTime.UtcNow) // YENI
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Notification>> GetByUserIdAsync(
            int userId,
            bool? isRead,
            int? type = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Notification> query = _context.Notifications
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Where(x => !x.ExpiresAt.HasValue || x.ExpiresAt > DateTime.UtcNow); // YENI

            if (isRead.HasValue)
            {
                query = query.Where(x => x.IsRead == isRead.Value);
            }

            if (type.HasValue && Enum.IsDefined(typeof(NotificationType), type.Value))
            {
                var parsedType = (NotificationType)type.Value;
                query = query.Where(x => x.Type == parsedType);
            }

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Notification>> GetUnreadByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications
                .Where(x => x.UserId == userId && !x.IsRead)
                .Where(x => !x.ExpiresAt.HasValue || x.ExpiresAt > DateTime.UtcNow) // YENI
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetUnreadCountByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications
                .CountAsync(
                    x => x.UserId == userId
                         && !x.IsRead
                         && (!x.ExpiresAt.HasValue || x.ExpiresAt > DateTime.UtcNow),
                    cancellationToken); // YENI
        }

        public async Task<List<Notification>> GetLatestByUserIdAsync(
            int userId,
            int take,
            CancellationToken cancellationToken = default)
        {
            if (take <= 0)
            {
                take = 5;
            }

            return await _context.Notifications
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Where(x => !x.ExpiresAt.HasValue || x.ExpiresAt > DateTime.UtcNow) // YENI
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            await _context.Notifications.AddAsync(notification, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<Notification> notifications, CancellationToken cancellationToken = default)
        {
            await _context.Notifications.AddRangeAsync(notifications, cancellationToken);
        }

        public void Update(Notification notification)
        {
            _context.Notifications.Update(notification);
        }

        public void Remove(Notification notification)
        {
            _context.Notifications.Remove(notification);
        }
    }
}
