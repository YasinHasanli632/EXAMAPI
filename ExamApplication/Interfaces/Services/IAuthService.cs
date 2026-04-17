using ExamApplication.DTO.Auth;
using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    /// <summary>
    /// Sistem daxilində authentication (login və token əməliyyatları)
    /// ilə bağlı business logic-i idarə edən servis kontraktıdır.
    /// </summary>
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(
            LoginRequestDto request,
            CancellationToken cancellationToken = default);

        Task<CurrentUserDto> GetCurrentUserAsync(
            CancellationToken cancellationToken = default);

        // YENI
        Task<AccessTokenResultDto> GenerateJwtTokenAsync(
            User user,
            CancellationToken cancellationToken = default);

        // YENI
        Task<LoginResponseDto> RefreshTokenAsync(
            RefreshTokenRequestDto request,
            CancellationToken cancellationToken = default);

        // YENI
        Task RevokeRefreshTokenAsync(
            RevokeRefreshTokenRequestDto request,
            CancellationToken cancellationToken = default);

        Task ChangePasswordAsync(
            ChangePasswordDto request,
            CancellationToken cancellationToken = default);

        Task ForgotPasswordAsync(
            ForgotPasswordDto request,
            CancellationToken cancellationToken = default);

        Task<VerifyResetOtpResponseDto> VerifyResetOtpAsync(
            VerifyResetOtpDto request,
            CancellationToken cancellationToken = default);

        Task ResetPasswordAsync(
            ResetPasswordDto request,
            CancellationToken cancellationToken = default);
    }
}

