using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.ValueObjects
{
    /// <summary>
    /// JWT token içində saxlanılan istifadəçi məlumatlarını daşıyır.
    /// </summary>
    public class JwtUserInfo
    {
        /// <summary>
        /// İstifadəçinin unikal Id-si.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// İstifadəçinin username-i.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// İstifadəçinin email ünvanı.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// İstifadəçinin sistemdəki rolu.
        /// </summary>
        public string Role { get; set; } = string.Empty;
    }
}
