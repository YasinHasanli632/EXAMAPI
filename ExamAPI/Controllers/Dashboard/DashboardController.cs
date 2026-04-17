using ExamApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExamAPI.Controllers.Dashboard
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("me")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher,Student")]
        public async Task<IActionResult> GetMyDashboard(CancellationToken cancellationToken)
        {
            var result = await _dashboardService.GetMyDashboardAsync(cancellationToken);
            return Ok(result);
        }
    }
}
