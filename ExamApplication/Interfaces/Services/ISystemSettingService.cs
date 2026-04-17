using ExamApplication.DTO.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    public interface ISystemSettingService
    {
        Task<SystemSettingDto> GetAsync(CancellationToken cancellationToken = default);
        Task<SystemSettingDto> UpdateAsync(UpdateSystemSettingDto request, CancellationToken cancellationToken = default);
        Task<SystemSettingDto> ResetToDefaultsAsync(CancellationToken cancellationToken = default);
    }
}
