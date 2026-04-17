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
    public class ExamSecurityLogRepository : IExamSecurityLogRepository
    {
        private readonly AppDbContext _context;

        public ExamSecurityLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ExamSecurityLog entity, CancellationToken cancellationToken = default)
        {
            await _context.ExamSecurityLogs.AddAsync(entity, cancellationToken);
        }

        public async Task<List<ExamSecurityLog>> GetByStudentExamIdAsync(int studentExamId, CancellationToken cancellationToken = default)
        {
            return await _context.ExamSecurityLogs
                .Where(x => x.StudentExamId == studentExamId)
                .OrderByDescending(x => x.OccurredAt)
                .ToListAsync(cancellationToken);
        }
    }
}
