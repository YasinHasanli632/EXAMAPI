using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface IExamSecurityLogRepository
    {
        Task AddAsync(ExamSecurityLog entity, CancellationToken cancellationToken = default);

        // YENI
        Task<List<ExamSecurityLog>> GetByStudentExamIdAsync(int studentExamId, CancellationToken cancellationToken = default);
    }
}
