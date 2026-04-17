using ExamApplication.DTO.Auth;
using ExamApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExamAPI.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _authService.LoginAsync(request, cancellationToken);
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

        // YENI
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(
            [FromBody] RefreshTokenRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(request, cancellationToken);
                return Ok(response);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        // YENI
        [HttpPost("revoke-refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RevokeRefreshToken(
            [FromBody] RevokeRefreshTokenRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _authService.RevokeRefreshTokenAsync(request, cancellationToken);
                return Ok(new { message = "Refresh token deaktiv edildi." });
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

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            try
            {
                var response = await _authService.GetCurrentUserAsync(cancellationToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _authService.ChangePasswordAsync(request, cancellationToken);
                return Ok(new { message = "Şifrə uğurla dəyişdirildi." });
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

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _authService.ForgotPasswordAsync(request, cancellationToken);

                return Ok(new
                {
                    message = "Təsdiq kodu email ünvanına göndərildi."
                });
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

        [HttpPost("verify-reset-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyResetOtp(
            [FromBody] VerifyResetOtpDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _authService.VerifyResetOtpAsync(request, cancellationToken);
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

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _authService.ResetPasswordAsync(request, cancellationToken);
                return Ok(new { message = "Şifrə uğurla yeniləndi." });
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
    }
}

