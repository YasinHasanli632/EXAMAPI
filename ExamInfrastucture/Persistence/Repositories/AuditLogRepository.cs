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
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _context;

        public AuditLogRepository(AppDbContext context)
        {
            _context = context;
        }

        // Id-yə görə audit log gətirir
        public async Task<AuditLog?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.AuditLogs
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // Bütün audit log-ları gətirir
        public async Task<List<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.AuditLogs
                .OrderByDescending(x => x.ActionTime)
                .ToListAsync(cancellationToken);
        }

        // Verilən user-ə aid audit log-ları gətirir
        public async Task<List<AuditLog>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.AuditLogs
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.ActionTime)
                .ToListAsync(cancellationToken);
        }

        // Verilən entity adı üzrə audit log-ları gətirir
        public async Task<List<AuditLog>> GetByEntityNameAsync(string entityName, CancellationToken cancellationToken = default)
        {
            return await _context.AuditLogs
                .Where(x => x.EntityName == entityName)
                .OrderByDescending(x => x.ActionTime)
                .ToListAsync(cancellationToken);
        }

        // Yeni audit log əlavə edir
        public async Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
        {
            await _context.AuditLogs.AddAsync(auditLog, cancellationToken);
        }
    }
}
