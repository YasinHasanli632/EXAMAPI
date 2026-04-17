using ExamApplication.DTO.User;
using ExamApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExamAPI.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Yeni user yaradır - yalnız SuperAdmin
        [HttpPost]
        [Authorize(Roles = "IsSuperAdmin")]
        public async Task<IActionResult> Create(
            [FromBody] CreateUserRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _userService.CreateAsync(request, cancellationToken);
                return Ok(response);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Bütün user-ləri qaytarır - Admin və SuperAdmin
        [HttpGet]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                var response = await _userService.GetAllAsync(cancellationToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Rola görə user-ləri qaytarır - Admin və SuperAdmin
        [HttpGet("by-role/{role}")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> GetByRole(
            [FromRoute] string role,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _userService.GetByRoleAsync(role, cancellationToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Id-yə görə user detail - Admin və SuperAdmin
        [HttpGet("{userId:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> GetById(
            [FromRoute] int userId,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _userService.GetByIdAsync(userId, cancellationToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // User update - Admin və SuperAdmin
        [HttpPut]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> Update(
            [FromBody] UpdateUserRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _userService.UpdateAsync(request, cancellationToken);
                return Ok(response);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Status dəyiş - yalnız SuperAdmin
        [HttpPatch("change-status")]
        [Authorize(Roles = "IsSuperAdmin")]
        public async Task<IActionResult> ChangeStatus(
            [FromBody] ChangeUserStatusRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var currentUserIdClaim =
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                    User.FindFirst("nameid")?.Value ??
                    User.FindFirst("sub")?.Value;

                var currentUserRole =
                    User.FindFirst(ClaimTypes.Role)?.Value ??
                    User.FindFirst("role")?.Value;

                if (string.IsNullOrWhiteSpace(currentUserIdClaim) || !int.TryParse(currentUserIdClaim, out var currentUserId))
                {
                    return Unauthorized(new { message = "Cari istifadəçi identifikasiyası alınmadı." });
                }

                if (string.IsNullOrWhiteSpace(currentUserRole))
                {
                    return Unauthorized(new { message = "Cari istifadəçi rolu alınmadı." });
                }

                await _userService.ChangeStatusAsync(
                    request,
                    currentUserId,
                    currentUserRole,
                    cancellationToken);

                return Ok(new { message = "İstifadəçi statusu uğurla yeniləndi" });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Soft delete - yalnız SuperAdmin
        [HttpDelete("{userId:int}")]
        [Authorize(Roles = "IsSuperAdmin")]
        public async Task<IActionResult> Delete(
            [FromRoute] int userId,
            CancellationToken cancellationToken)
        {
            try
            {
                await _userService.DeleteAsync(userId, cancellationToken);
                return Ok(new { message = "İstifadəçi uğurla silindi" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}