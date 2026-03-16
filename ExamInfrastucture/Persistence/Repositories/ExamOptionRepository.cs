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
    public class ExamOptionRepository : IExamOptionRepository
    {
        private readonly AppDbContext _context;

        public ExamOptionRepository(AppDbContext context)
        {
            _context = context;
        }

        // Id-yə görə variant gətirir
        public async Task<ExamOption?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ExamOptions
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // Suala aid bütün variantları gətirir
        public async Task<List<ExamOption>> GetByQuestionIdAsync(int questionId, CancellationToken cancellationToken = default)
        {
            return await _context.ExamOptions
                .Where(x => x.ExamQuestionId == questionId)
                .OrderBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        // Yeni variant əlavə edir
        public async Task AddAsync(ExamOption option, CancellationToken cancellationToken = default)
        {
            await _context.ExamOptions.AddAsync(option, cancellationToken);
        }

        // Variantı update üçün context-ə işarələyir
        public void Update(ExamOption option)
        {
            _context.ExamOptions.Update(option);
        }

        // Variantı silmək üçün context-ə işarələyir
        public void Remove(ExamOption option)
        {
            _context.ExamOptions.Remove(option);
        }
    }
}
