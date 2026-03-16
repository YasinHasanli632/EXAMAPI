using ExamApplication.DTO.User;
using ExamApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        // Yeni user yaradır.
        [HttpPost]
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
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Bütün user-ləri qaytarır.
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                var response = await _userService.GetAllAsync(cancellationToken);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Rola görə user-ləri qaytarır.
        [HttpGet("by-role/{role}")]
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
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Id-yə görə user detail məlumatını qaytarır.
        [HttpGet("{userId:int}")]
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
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        // Mövcud user məlumatlarını yeniləyir.
        [HttpPut]
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
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // User-in aktiv və ya passiv statusunu dəyişir.
        [HttpPatch("change-status")]
        public async Task<IActionResult> ChangeStatus(
            [FromBody] ChangeUserStatusRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _userService.ChangeStatusAsync(request, cancellationToken);

                return Ok(new
                {
                    message = "İstifadəçi statusu uğurla yeniləndi"
                });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // User-i soft delete məntiqi ilə passiv edir.
        [HttpDelete("{userId:int}")]
        public async Task<IActionResult> Delete(
            [FromRoute] int userId,
            CancellationToken cancellationToken)
        {
            try
            {
                await _userService.DeleteAsync(userId, cancellationToken);

                return Ok(new
                {
                    message = "İstifadəçi uğurla silindi"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
