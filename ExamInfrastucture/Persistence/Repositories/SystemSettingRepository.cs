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
    public class SystemSettingRepository : ISystemSettingRepository
    {
        private readonly AppDbContext _context;

        public SystemSettingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SystemSetting?> GetSingleAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SystemSettings
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddAsync(SystemSetting entity, CancellationToken cancellationToken = default)
        {
            await _context.SystemSettings.AddAsync(entity, cancellationToken);
        }

        public void Update(SystemSetting entity)
        {
            _context.SystemSettings.Update(entity);
        }
    }
}
