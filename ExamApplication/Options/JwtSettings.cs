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
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Token issuer dəyəri.
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Token audience dəyəri.
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Token-in neçə dəqiqə aktiv qalacağını göstərir.
        /// </summary>
        public int ExpirationMinutes { get; set; }
    }
}
