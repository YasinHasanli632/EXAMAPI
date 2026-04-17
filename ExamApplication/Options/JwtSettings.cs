using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Options
{
    /// <summary>
    /// JWT token yaratmaq üçün lazım olan konfiqurasiya dəyərlərini saxlayır.
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Token imzalama üçün gizli açar.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Token issuer dəyəri.
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Token audience dəyəri.
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Access token-in neçə dəqiqə aktiv qalacağını göstərir.
        /// </summary>
        public int ExpirationMinutes { get; set; }

        // YENI
        /// <summary>
        /// Refresh token-in neçə gün aktiv qalacağını göstərir.
        /// </summary>
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}
