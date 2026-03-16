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
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // Id-yə görə user gətirir
        public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // Username-ə görə user gətirir
        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
        }

        // Email-ə görə user gətirir
        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        }

        // Username-ə görə user-i detail-lərlə gətirir
        public async Task<User?> GetByUsernameWithDetailsAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(x => x.Student)
                .Include(x => x.Teacher)
                .Include(x => x.Notifications)
                .FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
        }

        // Id-yə görə user-i detail-lərlə gətirir
        public async Task<User?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(x => x.Student)
                .Include(x => x.Teacher)
                .Include(x => x.Notifications)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // Bütün user-ləri gətirir
        public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .OrderBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        // Username artıq mövcuddurmu yoxlayır
        public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AnyAsync(x => x.Username == username, cancellationToken);
        }

        // Email artıq mövcuddurmu yoxlayır
        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AnyAsync(x => x.Email == email, cancellationToken);
        }

        // Yeni user əlavə edir
        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }

        // Mövcud user-i update üçün context-ə işarələyir
        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        // User-i silmək üçün context-ə işarələyir
        public void Remove(User user)
        {
            _context.Users.Remove(user);
        }
    }
}
