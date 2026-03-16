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
    public class TeacherRepository : ITeacherRepository
    {
        private readonly AppDbContext _context;

        public TeacherRepository(AppDbContext context)
        {
            _context = context;
        }

        // Id-yə görə teacher gətirir
        public async Task<Teacher?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Teachers
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // UserId-yə görə teacher gətirir
        public async Task<Teacher?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Teachers
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        }

        // Id-yə görə teacher-i detail-lərlə gətirir
        public async Task<Teacher?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Teachers
                .Include(x => x.User)
                .Include(x => x.TeacherSubjects)
                    .ThenInclude(x => x.Subject)
                .Include(x => x.CreatedExams)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // Bütün teacher-ləri gətirir
        public async Task<List<Teacher>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Teachers
                .Include(x => x.User)
                .OrderBy(x => x.FullName)
                .ToListAsync(cancellationToken);
        }

        // Bütün teacher-ləri subject detail-ləri ilə gətirir
        public async Task<List<Teacher>> GetAllWithSubjectsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Teachers
                .Include(x => x.User)
                .Include(x => x.TeacherSubjects)
                    .ThenInclude(x => x.Subject)
                .OrderBy(x => x.FullName)
                .ToListAsync(cancellationToken);
        }

        // Yeni teacher əlavə edir
        public async Task AddAsync(Teacher teacher, CancellationToken cancellationToken = default)
        {
            await _context.Teachers.AddAsync(teacher, cancellationToken);
        }

        // Teacher-i update üçün context-ə işarələyir
        public void Update(Teacher teacher)
        {
            _context.Teachers.Update(teacher);
        }

        // Teacher-i silmək üçün context-ə işarələyir
        public void Remove(Teacher teacher)
        {
            _context.Teachers.Remove(teacher);
        }
    }
}
