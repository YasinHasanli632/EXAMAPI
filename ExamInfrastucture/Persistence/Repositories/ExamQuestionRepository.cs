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
    public class ExamQuestionRepository : IExamQuestionRepository
    {
        private readonly AppDbContext _context;

        public ExamQuestionRepository(AppDbContext context)
        {
            _context = context;
        }

        // Id-yə görə sual gətirir
        public async Task<ExamQuestion?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ExamQuestions
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // Id-yə görə sualı variantları ilə gətirir
        public async Task<ExamQuestion?> GetByIdWithOptionsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ExamQuestions
                .Include(x => x.Options)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // İmtahana aid bütün sualları gətirir
        public async Task<List<ExamQuestion>> GetByExamIdAsync(int examId, CancellationToken cancellationToken = default)
        {
            return await _context.ExamQuestions
                .Include(x => x.Options)
                .Where(x => x.ExamId == examId)
                .OrderBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        // Yeni sual əlavə edir
        public async Task AddAsync(ExamQuestion question, CancellationToken cancellationToken = default)
        {
            await _context.ExamQuestions.AddAsync(question, cancellationToken);
        }

        // Sualı update üçün context-ə işarələyir
        public void Update(ExamQuestion question)
        {
            _context.ExamQuestions.Update(question);
        }

        // Sualı silmək üçün context-ə işarələyir
        public void Remove(ExamQuestion question)
        {
            _context.ExamQuestions.Remove(question);
        }
    }
}
