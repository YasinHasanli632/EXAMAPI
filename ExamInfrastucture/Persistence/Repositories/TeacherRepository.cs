using ExamApplication.Interfaces.Repository;
using ExamDomain.Entities;
using ExamInfrastucture.DAL;
using Microsoft.EntityFrameworkCore;

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

        // Id-yə görə teacher-i bütün detail-lərlə gətirir
        public async Task<Teacher?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Teachers
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.TeacherSubjects)
                    .ThenInclude(x => x.Subject)
                .Include(x => x.CreatedExams)
                .Include(x => x.Tasks)
                .Include(x => x.ClassTeacherSubjects)
                    .ThenInclude(x => x.Subject)
                .Include(x => x.ClassTeacherSubjects)
                    .ThenInclude(x => x.ClassRoom)
                        .ThenInclude(x => x.StudentClasses)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // UserId-yə görə teacher-i bütün detail-lərlə gətirir
        public async Task<Teacher?> GetByUserIdWithDetailsAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Teachers
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.TeacherSubjects)
                    .ThenInclude(x => x.Subject)
                .Include(x => x.CreatedExams)
                .Include(x => x.Tasks)
                .Include(x => x.ClassTeacherSubjects)
                    .ThenInclude(x => x.Subject)
                .Include(x => x.ClassTeacherSubjects)
                    .ThenInclude(x => x.ClassRoom)
                        .ThenInclude(x => x.StudentClasses)
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        }

        // Bütün teacher-ləri sadə siyahı üçün gətirir
        public async Task<List<Teacher>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Teachers
                .AsNoTracking()
                .Include(x => x.User)
                .OrderBy(x => x.FullName)
                .ToListAsync(cancellationToken);
        }

        // Bütün teacher-ləri subject detail-ləri ilə gətirir
        public async Task<List<Teacher>> GetAllWithSubjectsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Teachers
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.TeacherSubjects)
                    .ThenInclude(x => x.Subject)
                .OrderBy(x => x.FullName)
                .ToListAsync(cancellationToken);
        }

        // Bütün teacher-ləri bütün detail-lərlə gətirir
        public async Task<List<Teacher>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Teachers
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.TeacherSubjects)
                    .ThenInclude(x => x.Subject)
                .Include(x => x.CreatedExams)
                .Include(x => x.Tasks)
                .Include(x => x.ClassTeacherSubjects)
                    .ThenInclude(x => x.Subject)
                .Include(x => x.ClassTeacherSubjects)
                    .ThenInclude(x => x.ClassRoom)
                        .ThenInclude(x => x.StudentClasses)
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

        // YENI
        // Teacher detail page üçün full dashboard datası
        public async Task<Teacher?> GetByIdWithFullDashboardAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Teachers
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.TeacherSubjects)
                    .ThenInclude(x => x.Subject)
                .Include(x => x.ClassTeacherSubjects)
                    .ThenInclude(x => x.Subject)
                .Include(x => x.ClassTeacherSubjects)
                    .ThenInclude(x => x.ClassRoom)
                        .ThenInclude(x => x.StudentClasses)
                .Include(x => x.CreatedExams)
                    .ThenInclude(x => x.Subject)
                .Include(x => x.CreatedExams)
                    .ThenInclude(x => x.StudentExams)
                .Include(x => x.Tasks)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // YENI
        public async Task<Teacher?> GetByUserIdWithFullDashboardAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Teachers
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.TeacherSubjects)
                    .ThenInclude(x => x.Subject)
                .Include(x => x.ClassTeacherSubjects)
                    .ThenInclude(x => x.Subject)
                .Include(x => x.ClassTeacherSubjects)
                    .ThenInclude(x => x.ClassRoom)
                        .ThenInclude(x => x.StudentClasses)
                .Include(x => x.CreatedExams)
                    .ThenInclude(x => x.Subject)
                .Include(x => x.CreatedExams)
                    .ThenInclude(x => x.StudentExams)
                .Include(x => x.Tasks)
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        }

        // YENI
        public async Task<List<Teacher>> GetByIdsWithDetailsAsync(List<int> teacherIds, CancellationToken cancellationToken = default)
        {
            if (teacherIds == null || teacherIds.Count == 0)
                return new List<Teacher>();

            return await _context.Teachers
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.TeacherSubjects)
                    .ThenInclude(x => x.Subject)
                .Include(x => x.Tasks)
                .Include(x => x.ClassTeacherSubjects)
                    .ThenInclude(x => x.Subject)
                .Include(x => x.ClassTeacherSubjects)
                    .ThenInclude(x => x.ClassRoom)
                        .ThenInclude(x => x.StudentClasses)
                .Where(x => teacherIds.Contains(x.Id))
                .OrderBy(x => x.FullName)
                .ToListAsync(cancellationToken);
        }

        // YENI
        public async Task<bool> ExistsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Teachers
                .AnyAsync(x => x.UserId == userId, cancellationToken);
        }
    }
}