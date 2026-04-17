using ExamApplication.DTO.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<DashboardResponseDto> GetMyDashboardAsync(CancellationToken cancellationToken = default);
    }
}
