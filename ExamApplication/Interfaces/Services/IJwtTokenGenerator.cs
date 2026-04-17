using ExamApplication.DTO.Auth;
using ExamDomain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    /// <summary>
    /// JWT token yaratmaq üçün istifadə olunan kontraktdır.
    /// </summary>
    public interface IJwtTokenGenerator
    {
        // YENI
        /// <summary>
        /// Verilmiş istifadəçi məlumatlarına əsasən JWT access token yaradır.
        /// </summary>
        AccessTokenResultDto GenerateAccessToken(JwtUserInfo userInfo);

        // YENI
        /// <summary>
        /// Random refresh token string yaradır.
        /// </summary>
        string GenerateRefreshToken();

        // YENI
        /// <summary>
        /// Expired access token içindən claim-ləri çıxarmaq üçün istifadə olunur.
        /// Refresh endpoint-də lazımdır.
        /// </summary>
        ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken);

        // YENI
        /// <summary>
        /// Refresh token expiry tarixi qaytarır.
        /// </summary>
        DateTime GetRefreshTokenExpiresAtUtc();
    }
}
