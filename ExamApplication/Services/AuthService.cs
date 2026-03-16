using ExamApplication.DTO.Auth;
using ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Services;
using ExamDomain.Entities;
using ExamDomain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Services
{
    /// <summary>
    /// Authentication ilə bağlı business logic-i idarə edir.
    /// Login, current user və JWT token yaratma əməliyyatları burada yerinə yetirilir.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ICurrentUserService _currentUserService;

        public AuthService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// İstifadəçinin login məlumatlarını yoxlayır.
        /// Username və ya email ilə user tapılır, password təsdiqlənir,
        /// sonra JWT token yaradılıb login cavabı qaytarılır.
        /// </summary>
        // <param name="request">Login üçün gələn request modeli</param>
        // <param name="cancellationToken">Async əməliyyat üçün cancellation token</param>
        /// <returns>Access token və login olmuş istifadəçi məlumatları</returns>
        // <exception cref="ArgumentNullException">Request null olduqda atılır</exception>
        // <exception cref="Exception">User tapılmadıqda, passiv olduqda və ya şifrə səhv olduqda atılır</exception>
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

            // İstifadəçi username ilə daxil olubsa əvvəl username üzrə axtarılır,
            // əgər @ varsa email kimi də yoxlanılır.
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

                // Username ilə tapılmasa email olaraq da yoxlamaq olar
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

            // Şifrənin doğruluğunu yoxlayır
            var isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

            if (!isPasswordValid)
                throw new Exception("Username/email və ya şifrə yanlışdır.");

            // JWT token yaradır
            var accessToken = await GenerateJwtTokenAsync(user, cancellationToken);

            // Login olan user-in tam məlumatını detail ilə çəkirik ki,
            // full name Student/Teacher profilindən düzgün qurulsun.
            var detailedUser = await _unitOfWork.Users.GetByIdWithDetailsAsync(user.Id, cancellationToken);

            if (detailedUser == null)
                throw new Exception("İstifadəçi məlumatları tam yüklənə bilmədi.");

            var response = new LoginResponseDto
            {
                AccessToken = accessToken,
                ExpiresAt = DateTime.UtcNow.AddHours(3), // burada token expiry ilə eyni saxla
                User = new CurrentUserDto
                {
                    UserId = detailedUser.Id,
                    Username = detailedUser.Username,
                    Email = detailedUser.Email,
                    Role = detailedUser.Role.ToString(),
                    IsActive = detailedUser.IsActive,
                    FullName = GetUserFullName(detailedUser)
                }
            };

            return response;
        }

        /// <summary>
        /// Hal-hazırda login olmuş istifadəçinin məlumatlarını qaytarır.
        /// User məlumatı token içindən çıxan user id əsasında database-dən götürülür.
        /// </summary>
        // <param name="cancellationToken">Async əməliyyat üçün cancellation token</param>
        /// <returns>Login olmuş istifadəçinin məlumatları</returns>
        /// <exception cref="Exception">Token içində user məlumatı olmadıqda və ya user tapılmadıqda atılır</exception>
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

        /// <summary>
        /// Verilmiş istifadəçi üçün JWT access token yaradır.
        /// Token içində user id, username və role kimi məlumatlar yerləşdirilir.
        /// </summary>
        /// <param name="user">Token yaradılacaq istifadəçi</param>
        /// <param name="cancellationToken">Async əməliyyat üçün cancellation token</param>
        /// <returns>JWT token string</returns>
        /// <exception cref="ArgumentNullException">User null olduqda atılır</exception>
        public Task<string> GenerateJwtTokenAsync(
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

            var token = _jwtTokenGenerator.GenerateToken(jwtUserInfo);

            return Task.FromResult(token);
        }

        /// <summary>
        /// İstifadəçinin ekranda göstəriləcək tam adını qaytarır.
        /// Əgər student profili varsa student full name,
        /// teacher profili varsa teacher full name,
        /// heç biri yoxdursa username qaytarılır.
        /// </summary>
        /// <param name="user">Tam adı çıxarılacaq user</param>
        /// <returns>İstifadəçi üçün uyğun display full name</returns>
        private static string GetUserFullName(User user)
        {
            if (user.Student != null && !string.IsNullOrWhiteSpace(user.Student.FullName))
                return user.Student.FullName;

            if (user.Teacher != null && !string.IsNullOrWhiteSpace(user.Teacher.FullName))
                return user.Teacher.FullName;

            return user.Username;
        }
    }
}
