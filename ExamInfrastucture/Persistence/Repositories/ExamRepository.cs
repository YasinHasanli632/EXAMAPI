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
    public class ExamRepository : IExamRepository
    {
        private readonly AppDbContext _context;

        public ExamRepository(AppDbContext context)
        {
            _context = context;
        }

        // Id-yə görə imtahan gətirir
        public async Task<Exam?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // Id-yə görə imtahanı bütün detail-lərlə gətirir
        public async Task<Exam?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Include(x => x.Questions)
                    .ThenInclude(x => x.Options)
                .Include(x => x.StudentExams)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // Bütün imtahanları gətirir
        public async Task<List<Exam>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                .OrderByDescending(x => x.StartTime)
                .ToListAsync(cancellationToken);
        }

        // Müəllimə aid imtahanları gətirir
        public async Task<List<Exam>> GetByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(x => x.Subject)
                .Where(x => x.TeacherId == teacherId)
                .OrderByDescending(x => x.StartTime)
                .ToListAsync(cancellationToken);
        }

        // Fənnə aid imtahanları gətirir
        public async Task<List<Exam>> GetBySubjectIdAsync(int subjectId, CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(x => x.Teacher)
                .Where(x => x.SubjectId == subjectId)
                .OrderByDescending(x => x.StartTime)
                .ToListAsync(cancellationToken);
        }

        // Aktiv imtahanları gətirir
        public async Task<List<Exam>> GetActiveExamsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                .Where(x => x.Status == ExamStatus.Active)
                .OrderBy(x => x.StartTime)
                .ToListAsync(cancellationToken);
        }

        // Yeni imtahan əlavə edir
        public async Task AddAsync(Exam exam, CancellationToken cancellationToken = default)
        {
            await _context.Exams.AddAsync(exam, cancellationToken);
        }

        // İmtahanı update üçün context-ə işarələyir
        public void Update(Exam exam)
        {
            _context.Exams.Update(exam);
        }

        // İmtahanı silmək üçün context-ə işarələyir
        public void Remove(Exam exam)
        {
            _context.Exams.Remove(exam);
        }
    }
}
