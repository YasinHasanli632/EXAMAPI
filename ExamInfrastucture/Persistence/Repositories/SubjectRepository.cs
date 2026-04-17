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
    public class SubjectRepository : ISubjectRepository
    {
        private readonly AppDbContext _context;

        public SubjectRepository(AppDbContext context)
        {
            _context = context;
        }

        // Id-yə görə fənn gətirir
        public async Task<Subject?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Subjects
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // YENI
        public async Task<Subject?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Subjects
                .AsNoTracking()
                .Include(x => x.TeacherSubjects)
                    .ThenInclude(x => x.Teacher)
                        .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // Bütün fənləri gətirir
        public async Task<List<Subject>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Subjects
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        // YENI
        public async Task<List<Subject>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Subjects
                .AsNoTracking()
                .Include(x => x.TeacherSubjects)
                    .ThenInclude(x => x.Teacher)
                        .ThenInclude(x => x.User)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        // Müəllimə aid fənləri gətirir
        public async Task<List<Subject>> GetSubjectsByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default)
        {
            return await _context.TeacherSubjects
                .Where(x => x.TeacherId == teacherId)
                .Select(x => x.Subject)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        // Fənn adının mövcud olub-olmadığını yoxlayır
        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Subjects
                .AnyAsync(x => x.Name == name, cancellationToken);
        }

        // YENI
        public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.Subjects
                .AnyAsync(x => x.Code != null && x.Code == code, cancellationToken);
        }

        // Yeni fənn əlavə edir
        public async Task AddAsync(Subject subject, CancellationToken cancellationToken = default)
        {
            await _context.Subjects.AddAsync(subject, cancellationToken);
        }

        // Fənni update üçün context-ə işarələyir
        public void Update(Subject subject)
        {
            _context.Subjects.Update(subject);
        }

        // Fənni silmək üçün context-ə işarələyir
        public void Remove(Subject subject)
        {
            _context.Subjects.Remove(subject);
        }

        // YENI
        public async Task<List<Subject>> GetByIdsAsync(List<int> subjectIds, CancellationToken cancellationToken = default)
        {
            if (subjectIds == null || subjectIds.Count == 0)
                return new List<Subject>();

            return await _context.Subjects
                .Where(x => subjectIds.Contains(x.Id))
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }
    }
}
