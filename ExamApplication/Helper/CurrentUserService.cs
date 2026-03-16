using ExamApplication.Interfaces.Services;
using ExamDomain.ValueObjects;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ExamApplication.Helper
{
    /// <summary>
    /// Cari request-in token məlumatlarından istifadəçi məlumatını çıxarır.
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Token içindən user məlumatlarını oxuyur və JwtUserInfo şəklində qaytarır.
        /// </summary>
        /// <returns>Hal-hazırda login olmuş istifadəçinin token məlumatları</returns>
        public JwtUserInfo? GetCurrentUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
                return null;

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var usernameClaim = user.FindFirst(ClaimTypes.Name)?.Value;
            var emailClaim = user.FindFirst(ClaimTypes.Email)?.Value;
            var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return null;

            if (!int.TryParse(userIdClaim, out var userId))
                return null;

            return new JwtUserInfo
            {
                UserId = userId,
                Username = usernameClaim ?? string.Empty,
                Email = emailClaim ?? string.Empty,
                Role = roleClaim ?? string.Empty
            };
        }
    }
}
