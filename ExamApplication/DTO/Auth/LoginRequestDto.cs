using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Auth
{
    /// <summary>
    /// İstifadəçinin login olmaq üçün göndərdiyi məlumatları saxlayır.
    /// Username və ya email + password əsasında giriş edilir.
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// İstifadəçinin username-i və ya email ünvanı.
        /// </summary>
        public string UsernameOrEmail { get; set; } = string.Empty;

        /// <summary>
        /// İstifadəçinin daxil etdiyi şifrə.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
