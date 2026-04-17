using ExamApplication.DTO.Auth;
using ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Services;
using ExamDomain.Entities;
using ExamDomain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Services
{
    /// <summary>
    /// Authentication ilə bağlı business logic-i idarə edir.
    /// Login, current user və JWT token yaratma əməliyyatları burada yerinə yetirilir.
    /// </summary>
    /// 



    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailService _emailService;

        public AuthService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            ICurrentUserService currentUserService,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _currentUserService = currentUserService;
            _emailService = emailService;
        }

        public async Task<LoginResponseDto> LoginAsync(
            LoginRequestDto request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.UsernameOrEmail))
                throw new Exception("Username və ya email boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new Exception("Şifrə boş ola bilməz.");

            User? user;

            if (request.UsernameOrEmail.Contains("@"))
            {
                user = await _unitOfWork.Users.GetByEmailAsync(
                    request.UsernameOrEmail.Trim(),
                    cancellationToken);
            }
            else
            {
                user = await _unitOfWork.Users.GetByUsernameAsync(
                    request.UsernameOrEmail.Trim(),
                    cancellationToken);

                if (user == null)
                {
                    user = await _unitOfWork.Users.GetByEmailAsync(
                        request.UsernameOrEmail.Trim(),
                        cancellationToken);
                }
            }

            if (user == null)
                throw new Exception("İstifadəçi tapılmadı.");

            if (!user.IsActive)
                throw new Exception("İstifadəçi passivdir. Sistemə giriş icazəsi yoxdur.");

            var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

            if (!isPasswordValid)
                throw new Exception("Username/email və ya şifrə yanlışdır.");

            var detailedUser = await _unitOfWork.Users.GetByIdWithDetailsAsync(user.Id, cancellationToken);

            if (detailedUser == null)
                throw new Exception("İstifadəçi məlumatları tam yüklənə bilmədi.");

            // YENI
            var response = await BuildLoginResponseAsync(detailedUser, cancellationToken);
            return response;
        }

        public async Task<CurrentUserDto> GetCurrentUserAsync(
            CancellationToken cancellationToken = default)
        {
            var jwtUserInfo = _currentUserService.GetCurrentUser();

            if (jwtUserInfo == null)
                throw new Exception("Cari istifadəçi məlumatı tapılmadı.");

            var user = await _unitOfWork.Users.GetByIdWithDetailsAsync(jwtUserInfo.UserId, cancellationToken);

            if (user == null)
                throw new Exception("İstifadəçi tapılmadı.");

            return new CurrentUserDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                FullName = GetUserFullName(user)
            };
        }

        // YENI
        public Task<AccessTokenResultDto> GenerateJwtTokenAsync(
            User user,
            CancellationToken cancellationToken = default)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var jwtUserInfo = new JwtUserInfo
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString()
            };

            var tokenResult = _jwtTokenGenerator.GenerateAccessToken(jwtUserInfo);
            return Task.FromResult(tokenResult);
        }

        // YENI
        public async Task<LoginResponseDto> RefreshTokenAsync(
            RefreshTokenRequestDto request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.AccessToken))
                throw new Exception("Access token boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                throw new Exception("Refresh token boş ola bilməz.");

            var principal = _jwtTokenGenerator.GetPrincipalFromExpiredToken(request.AccessToken);

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                throw new Exception("Access token içindən istifadəçi məlumatı oxunmadı.");

            var storedRefreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(
                request.RefreshToken,
                cancellationToken);

            if (storedRefreshToken == null)
                throw new Exception("Refresh token tapılmadı.");

            if (storedRefreshToken.UserId != userId)
                throw new Exception("Refresh token bu istifadəçiyə aid deyil.");

            if (!storedRefreshToken.IsActive)
                throw new Exception("Refresh token artıq aktiv deyil.");

            var user = await _unitOfWork.Users.GetByIdWithDetailsAsync(userId, cancellationToken);

            if (user == null)
                throw new Exception("İstifadəçi tapılmadı.");

            if (!user.IsActive)
                throw new Exception("İstifadəçi passivdir.");

            // YENI
            // Köhnə refresh token bir dəfə işlənəndən sonra deaktiv olunur.
            storedRefreshToken.RevokedAt = DateTime.UtcNow;
            storedRefreshToken.RevocationReason = "Refresh token yeniləndi.";

            var newRefreshTokenValue = _jwtTokenGenerator.GenerateRefreshToken();
            storedRefreshToken.ReplacedByToken = newRefreshTokenValue;

            _unitOfWork.RefreshTokens.Update(storedRefreshToken);

            var newRefreshToken = CreateRefreshTokenEntity(user.Id, newRefreshTokenValue);
            await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var accessTokenResult = await GenerateJwtTokenAsync(user, cancellationToken);

            return new LoginResponseDto
            {
                AccessToken = accessTokenResult.AccessToken,
                ExpiresAt = accessTokenResult.ExpiresAt,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiresAt = newRefreshToken.ExpiresAt,
                MustChangePassword = user.MustChangePassword,
                User = new CurrentUserDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    IsActive = user.IsActive,
                    FullName = GetUserFullName(user)
                }
            };
        }

        // YENI
        public async Task RevokeRefreshTokenAsync(
            RevokeRefreshTokenRequestDto request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                throw new Exception("Refresh token boş ola bilməz.");

            var storedRefreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(
                request.RefreshToken,
                cancellationToken);

            if (storedRefreshToken == null)
                return;

            if (storedRefreshToken.IsRevoked)
                return;

            storedRefreshToken.RevokedAt = DateTime.UtcNow;
            storedRefreshToken.RevocationReason = "İstifadəçi logout oldu.";

            _unitOfWork.RefreshTokens.Update(storedRefreshToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task ChangePasswordAsync(
            ChangePasswordDto request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                throw new Exception("Cari şifrə boş ola bilməz.");

            ValidateNewPassword(request.NewPassword, request.ConfirmNewPassword);

            var jwtUserInfo = _currentUserService.GetCurrentUser();

            if (jwtUserInfo == null)
                throw new Exception("Cari istifadəçi məlumatı tapılmadı.");

            var user = await _unitOfWork.Users.GetByIdAsync(jwtUserInfo.UserId, cancellationToken);

            if (user == null)
                throw new Exception("İstifadəçi tapılmadı.");

            var isCurrentPasswordValid = _passwordHasher.Verify(request.CurrentPassword, user.PasswordHash);

            if (!isCurrentPasswordValid)
                throw new Exception("Cari şifrə yanlışdır.");

            var isSamePassword = _passwordHasher.Verify(request.NewPassword, user.PasswordHash);

            if (isSamePassword)
                throw new Exception("Yeni şifrə cari şifrə ilə eyni ola bilməz.");

            user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
            user.MustChangePassword = false;

            user.PasswordResetOtp = null;
            user.PasswordResetOtpExpiresAt = null;
            user.PasswordResetOtpUsed = false;
            user.PasswordResetOtpAttemptCount = 0;
            user.PasswordResetRequestedAt = null;

            _unitOfWork.Users.Update(user);

            // YENI
            await RevokeAllUserRefreshTokensAsync(
                user.Id,
                "Şifrə dəyişdirildi. Köhnə refresh tokenlər deaktiv edildi.",
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task ForgotPasswordAsync(
            ForgotPasswordDto request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new Exception("Email boş ola bilməz.");

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var user = await _unitOfWork.Users.GetByEmailAsync(normalizedEmail, cancellationToken);

            if (user == null)
                throw new Exception("Bu email ilə istifadəçi tapılmadı.");

            var otp = new Random().Next(100000, 999999).ToString();

            user.PasswordResetOtp = otp;
            user.PasswordResetOtpExpiresAt = DateTime.UtcNow.AddMinutes(5);
            user.PasswordResetOtpUsed = false;
            user.PasswordResetOtpAttemptCount = 0;
            user.PasswordResetRequestedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _emailService.SendAsync(
                user.Email,
                "Şifrə yeniləmə təsdiq kodu",
                $"Sizin təsdiq kodunuz: {otp}",
                cancellationToken);
        }

        public async Task<VerifyResetOtpResponseDto> VerifyResetOtpAsync(
            VerifyResetOtpDto request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new Exception("Email boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(request.Otp))
                throw new Exception("OTP boş ola bilməz.");

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var user = await _unitOfWork.Users.GetByEmailAsync(normalizedEmail, cancellationToken);

            if (user == null)
                throw new Exception("İstifadəçi tapılmadı.");

            if (string.IsNullOrWhiteSpace(user.PasswordResetOtp))
                throw new Exception("Aktiv OTP kodu tapılmadı.");

            if (user.PasswordResetOtpUsed)
                throw new Exception("Bu OTP artıq istifadə olunub.");

            if (!user.PasswordResetOtpExpiresAt.HasValue || user.PasswordResetOtpExpiresAt.Value < DateTime.UtcNow)
                throw new Exception("OTP kodunun vaxtı bitib.");

            user.PasswordResetOtpAttemptCount++;

            if (user.PasswordResetOtp != request.Otp.Trim())
            {
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                throw new Exception("OTP kodu yanlışdır.");
            }

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new VerifyResetOtpResponseDto
            {
                IsValid = true,
                Message = "OTP kodu təsdiqləndi."
            };
        }

        public async Task ResetPasswordAsync(
            ResetPasswordDto request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new Exception("Email boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(request.Otp))
                throw new Exception("OTP boş ola bilməz.");

            ValidateNewPassword(request.NewPassword, request.ConfirmNewPassword);

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            var user = await _unitOfWork.Users.GetByEmailAsync(normalizedEmail, cancellationToken);

            if (user == null)
                throw new Exception("İstifadəçi tapılmadı.");

            if (string.IsNullOrWhiteSpace(user.PasswordResetOtp))
                throw new Exception("Aktiv OTP tapılmadı.");

            if (user.PasswordResetOtpUsed)
                throw new Exception("Bu OTP artıq istifadə olunub.");

            if (!user.PasswordResetOtpExpiresAt.HasValue || user.PasswordResetOtpExpiresAt.Value < DateTime.UtcNow)
                throw new Exception("OTP kodunun vaxtı bitib.");

            if (user.PasswordResetOtp != request.Otp.Trim())
                throw new Exception("OTP kodu yanlışdır.");

            var isSamePassword = _passwordHasher.Verify(request.NewPassword, user.PasswordHash);

            if (isSamePassword)
                throw new Exception("Yeni şifrə cari şifrə ilə eyni ola bilməz.");

            user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
            user.MustChangePassword = false;
            user.PasswordResetOtpUsed = true;
            user.PasswordResetOtp = null;
            user.PasswordResetOtpExpiresAt = null;
            user.PasswordResetOtpAttemptCount = 0;
            user.PasswordResetRequestedAt = null;

            _unitOfWork.Users.Update(user);

            // YENI
            await RevokeAllUserRefreshTokensAsync(
                user.Id,
                "Şifrə reset edildi. Köhnə refresh tokenlər deaktiv edildi.",
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // YENI
        private async Task<LoginResponseDto> BuildLoginResponseAsync(
            User user,
            CancellationToken cancellationToken)
        {
            var accessTokenResult = await GenerateJwtTokenAsync(user, cancellationToken);

            var refreshTokenValue = _jwtTokenGenerator.GenerateRefreshToken();
            var refreshToken = CreateRefreshTokenEntity(user.Id, refreshTokenValue);

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new LoginResponseDto
            {
                AccessToken = accessTokenResult.AccessToken,
                ExpiresAt = accessTokenResult.ExpiresAt,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAt = refreshToken.ExpiresAt,
                MustChangePassword = user.MustChangePassword,
                User = new CurrentUserDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    IsActive = user.IsActive,
                    FullName = GetUserFullName(user)
                }
            };
        }

        // YENI
        private RefreshToken CreateRefreshTokenEntity(int userId, string refreshTokenValue)
        {
            return new RefreshToken
            {
                UserId = userId,
                Token = refreshTokenValue,
                ExpiresAt = _jwtTokenGenerator.GetRefreshTokenExpiresAtUtc(),
                CreatedAt = DateTime.UtcNow
            };
        }

        // YENI
        private async Task RevokeAllUserRefreshTokensAsync(
            int userId,
            string reason,
            CancellationToken cancellationToken)
        {
            var tokens = await _unitOfWork.RefreshTokens.GetByUserIdAsync(userId, cancellationToken);

            var activeTokens = tokens
                .Where(x => x.IsActive)
                .ToList();

            if (activeTokens.Count == 0)
                return;

            foreach (var token in activeTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.RevocationReason = reason;
            }

            _unitOfWork.RefreshTokens.UpdateRange(activeTokens);
        }

        private static string GetUserFullName(User user)
        {
            if (user.Teacher != null && !string.IsNullOrWhiteSpace(user.Teacher.FullName))
                return user.Teacher.FullName;

            if (user.Student != null && !string.IsNullOrWhiteSpace(user.Student.FullName))
                return user.Student.FullName;

            return $"{user.FirstName} {user.LastName}".Trim();
        }

        private static void ValidateNewPassword(string newPassword, string confirmNewPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new Exception("Yeni şifrə boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(confirmNewPassword))
                throw new Exception("Təkrar şifrə boş ola bilməz.");

            if (newPassword != confirmNewPassword)
                throw new Exception("Yeni şifrə ilə təkrar şifrə eyni deyil.");

            if (newPassword.Length < 6)
                throw new Exception("Yeni şifrə minimum 6 simvol olmalıdır.");
        }
    }
}




