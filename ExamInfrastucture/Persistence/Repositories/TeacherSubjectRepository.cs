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
    public class TeacherSubjectRepository : ITeacherSubjectRepository
    {
        private readonly AppDbContext _context;

        public TeacherSubjectRepository(AppDbContext context)
        {
            _context = context;
        }

        // Id-yə görə teacher-subject əlaqəsini gətirir
        public async Task<TeacherSubject?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.TeacherSubjects
                .Include(x => x.Teacher)
                .Include(x => x.Subject)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // Müəllim və fənn cütünə görə əlaqəni gətirir
        public async Task<TeacherSubject?> GetByTeacherAndSubjectAsync(int teacherId, int subjectId, CancellationToken cancellationToken = default)
        {
            return await _context.TeacherSubjects
                .Include(x => x.Teacher)
                .Include(x => x.Subject)
                .FirstOrDefaultAsync(x => x.TeacherId == teacherId && x.SubjectId == subjectId, cancellationToken);
        }

        // Müəllimin bütün fənn əlaqələrini gətirir
        public async Task<List<TeacherSubject>> GetByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default)
        {
            return await _context.TeacherSubjects
                .Include(x => x.Subject)
                .Where(x => x.TeacherId == teacherId)
                .OrderBy(x => x.Subject.Name)
                .ToListAsync(cancellationToken);
        }

        // Fənn üzrə bütün müəllim əlaqələrini gətirir
        public async Task<List<TeacherSubject>> GetBySubjectIdAsync(int subjectId, CancellationToken cancellationToken = default)
        {
            return await _context.TeacherSubjects
                .Include(x => x.Teacher)
                .Where(x => x.SubjectId == subjectId)
                .OrderBy(x => x.Teacher.FullName)
                .ToListAsync(cancellationToken);
        }

        // Müəllim-fənn əlaqəsinin mövcud olub-olmadığını yoxlayır
        public async Task<bool> ExistsAsync(int teacherId, int subjectId, CancellationToken cancellationToken = default)
        {
            return await _context.TeacherSubjects
                .AnyAsync(x => x.TeacherId == teacherId && x.SubjectId == subjectId, cancellationToken);
        }

        // Yeni teacher-subject əlaqəsi əlavə edir
        public async Task AddAsync(TeacherSubject teacherSubject, CancellationToken cancellationToken = default)
        {
            await _context.TeacherSubjects.AddAsync(teacherSubject, cancellationToken);
        }

        // Teacher-subject əlaqəsini update üçün context-ə işarələyir
        public void Update(TeacherSubject teacherSubject)
        {
            _context.TeacherSubjects.Update(teacherSubject);
        }

        // Teacher-subject əlaqəsini silmək üçün context-ə işarələyir
        public void Remove(TeacherSubject teacherSubject)
        {
            _context.TeacherSubjects.Remove(teacherSubject);
        }

        // YENI
        public async Task<List<TeacherSubject>> GetByTeacherIdsAsync(List<int> teacherIds, CancellationToken cancellationToken = default)
        {
            if (teacherIds == null || teacherIds.Count == 0)
                return new List<TeacherSubject>();

            return await _context.TeacherSubjects
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                .Where(x => teacherIds.Contains(x.TeacherId))
                .OrderBy(x => x.TeacherId)
                .ThenBy(x => x.Subject.Name)
                .ToListAsync(cancellationToken);
        }

        // YENI
        public async Task<List<TeacherSubject>> GetByTeacherIdWithTeacherAsync(int teacherId, CancellationToken cancellationToken = default)
        {
            return await _context.TeacherSubjects
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Where(x => x.TeacherId == teacherId)
                .OrderBy(x => x.Subject.Name)
                .ToListAsync(cancellationToken);
        }

        // YENI
        public Task RemoveRangeAsync(List<TeacherSubject> teacherSubjects, CancellationToken cancellationToken = default)
        {
            _context.TeacherSubjects.RemoveRange(teacherSubjects);
            return Task.CompletedTask;
        }
    }
}
