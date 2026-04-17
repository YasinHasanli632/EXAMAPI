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
    public class StudentAnswerOptionRepository : IStudentAnswerOptionRepository
    {
        private readonly AppDbContext _context;

        public StudentAnswerOptionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<StudentAnswerOption>> GetByStudentAnswerIdAsync(int studentAnswerId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentAnswerOptions
                .Include(x => x.ExamOption)
                .Where(x => x.StudentAnswerId == studentAnswerId)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(StudentAnswerOption entity, CancellationToken cancellationToken = default)
        {
            await _context.StudentAnswerOptions.AddAsync(entity, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<StudentAnswerOption> entities, CancellationToken cancellationToken = default)
        {
            await _context.StudentAnswerOptions.AddRangeAsync(entities, cancellationToken);
        }

        public void RemoveRange(IEnumerable<StudentAnswerOption> entities)
        {
            _context.StudentAnswerOptions.RemoveRange(entities);
        }
    }
}
