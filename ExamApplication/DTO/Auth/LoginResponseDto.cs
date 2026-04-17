using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Auth
{
    /// <summary>
    /// Login əməliyyatı uğurlu olduqda frontend-ə qaytarılan cavab modelidir.
    /// JWT token və login olmuş istifadəçi məlumatlarını saxlayır.
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// İstifadəçi üçün yaradılmış JWT access token.
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Access token-in bitmə tarixini göstərir.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        // YENI
        /// <summary>
        /// Access token bitəndə yenisini almaq üçün istifadə olunan refresh token.
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        // YENI
        /// <summary>
        /// Refresh token-in bitmə tarixini göstərir.
        /// </summary>
        public DateTime RefreshTokenExpiresAt { get; set; }

        public bool MustChangePassword { get; set; }

        /// <summary>
        /// Login olmuş istifadəçinin əsas məlumatları.
        /// </summary>
        public CurrentUserDto User { get; set; } = new CurrentUserDto();
    }
}
