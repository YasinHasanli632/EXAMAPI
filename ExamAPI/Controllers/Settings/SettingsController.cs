using ExamApplication.DTO.Settings;
using ExamApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamAPI.Controllers.Settings
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SettingsController : ControllerBase
    {
        private readonly ISystemSettingService _systemSettingService;

        public SettingsController(ISystemSettingService systemSettingService)
        {
            _systemSettingService = systemSettingService;
        }

        // YENI:
        // Müəllim və tələbə də bu ayarları oxuya bilsin.
        // Bu, xüsusilə showScoreImmediately kimi UI davranışları üçün lazımdır.
        [HttpGet]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher,Student")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var result = await _systemSettingService.GetAsync(cancellationToken);
            return Ok(result);
        }

        // Admin-only qalır
        [HttpPut]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> Update(
            [FromBody] UpdateSystemSettingDto request,
            CancellationToken cancellationToken)
        {
            var result = await _systemSettingService.UpdateAsync(request, cancellationToken);
            return Ok(result);
        }

        // Admin-only qalır
        [HttpPost("reset-defaults")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> ResetDefaults(CancellationToken cancellationToken)
        {
            var result = await _systemSettingService.ResetToDefaultsAsync(cancellationToken);
            return Ok(result);
        }
    }
}