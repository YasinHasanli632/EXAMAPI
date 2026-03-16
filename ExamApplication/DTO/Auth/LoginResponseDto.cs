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
        /// Token-in bitmə tarixini göstərir.
        /// Frontend bu məlumatla session idarəsini daha rahat qura bilər.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Login olmuş istifadəçinin əsas məlumatları.
        /// </summary>
        public CurrentUserDto User { get; set; } = new CurrentUserDto();
    }
}
