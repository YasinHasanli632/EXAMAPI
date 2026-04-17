using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface ISystemSettingRepository
    {
        Task<SystemSetting?> GetSingleAsync(CancellationToken cancellationToken = default);
        Task AddAsync(SystemSetting entity, CancellationToken cancellationToken = default);
        void Update(SystemSetting entity);
    }
}
