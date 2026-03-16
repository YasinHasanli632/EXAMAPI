using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface IAuditLogRepository
    {
        Task<AuditLog?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<List<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<List<AuditLog>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

        Task<List<AuditLog>> GetByEntityNameAsync(string entityName, CancellationToken cancellationToken = default);

        Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    }
}
