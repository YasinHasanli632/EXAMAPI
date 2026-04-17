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
    public class ExamAccessCodeRepository : IExamAccessCodeRepository
    {
        private readonly AppDbContext _context;

        public ExamAccessCodeRepository(AppDbContext context)
        {
            _context = context;
        }

        // Id-yə görə access code gətirir
        public async Task<ExamAccessCode?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ExamAccessCodes
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // Koda görə access code gətirir
        public async Task<ExamAccessCode?> GetByCodeAsync(string accessCode, CancellationToken cancellationToken = default)
        {
            return await _context.ExamAccessCodes
                .FirstOrDefaultAsync(x => x.AccessCode == accessCode, cancellationToken);
        }

        // Exam və student cütünə görə access code gətirir
        public async Task<ExamAccessCode?> GetByExamAndStudentAsync(int examId, int studentId, CancellationToken cancellationToken = default)
        {
            return await _context.ExamAccessCodes
                .FirstOrDefaultAsync(x => x.ExamId == examId && x.StudentId == studentId, cancellationToken);
        }

        // İmtahana aid bütün access code-ları gətirir
        public async Task<List<ExamAccessCode>> GetByExamIdAsync(int examId, CancellationToken cancellationToken = default)
        {
            return await _context.ExamAccessCodes
                .Where(x => x.ExamId == examId)
                .OrderBy(x => x.StudentId)
                .ToListAsync(cancellationToken);
        }

        // Exam və student üçün kodun mövcud olub-olmadığını yoxlayır
        public async Task<bool> ExistsAsync(int examId, int studentId, CancellationToken cancellationToken = default)
        {
            return await _context.ExamAccessCodes
                .AnyAsync(x => x.ExamId == examId && x.StudentId == studentId, cancellationToken);
        }

        // Yeni access code əlavə edir
        public async Task<List<ExamAccessCode>> GetUnusedByExamIdAsync(int examId, CancellationToken cancellationToken = default)
        {
            return await _context.ExamAccessCodes
                .Where(x => x.ExamId == examId && !x.IsUsed)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ExamAccessCode>> GetExpiringCodesAsync(DateTime now, CancellationToken cancellationToken = default)
        {
            return await _context.ExamAccessCodes
                .Where(x => !x.IsUsed && x.ExpireAt <= now)
                .ToListAsync(cancellationToken);
        }
        public async Task AddAsync(ExamAccessCode examAccessCode, CancellationToken cancellationToken = default)
        {
            await _context.ExamAccessCodes.AddAsync(examAccessCode, cancellationToken);
        }

        // Access code-u update üçün context-ə işarələyir
        public void Update(ExamAccessCode examAccessCode)
        {
            _context.ExamAccessCodes.Update(examAccessCode);
        }

        // Access code-u silmək üçün context-ə işarələyir
        public void Remove(ExamAccessCode examAccessCode)
        {
            _context.ExamAccessCodes.Remove(examAccessCode);
        }
    }
}
