using ExamApplication.Interfaces.Repository;
using ExamDomain.Entities;
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

        // Id-yə görə notification gətirir
        public async Task<Notification?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // User-ə aid bütün notification-ları gətirir
        public async Task<List<Notification>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        // User-ə aid oxunmamış notification-ları gətirir
        public async Task<List<Notification>> GetUnreadByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications
                .Where(x => x.UserId == userId && !x.IsRead)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        // User-ə aid oxunmamış notification sayını gətirir
        public async Task<int> GetUnreadCountByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications
                .CountAsync(x => x.UserId == userId && !x.IsRead, cancellationToken);
        }

        // Yeni notification əlavə edir
        public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            await _context.Notifications.AddAsync(notification, cancellationToken);
        }

        // Notification-u update üçün context-ə işarələyir
        public void Update(Notification notification)
        {
            _context.Notifications.Update(notification);
        }

        // Notification-u silmək üçün context-ə işarələyir
        public void Remove(Notification notification)
        {
            _context.Notifications.Remove(notification);
        }
    }
}
