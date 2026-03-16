using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.User
{
    /// <summary>
    /// Admin paneldən yeni user yaratmaq üçün istifadə olunan request modelidir.
    /// Bu model əsas account məlumatlarını daşıyır.
    /// </summary>
    public class CreateUserRequestDto
    {
        /// <summary>
        /// İstifadəçinin username-i.
        /// Sistemdə unikal olmalıdır.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// İstifadəçinin email ünvanı.
        /// Sistemdə unikal olmalıdır.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// İstifadəçi üçün təyin ediləcək şifrə.
        /// Servis daxilində hash olunaraq saxlanılmalıdır.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// İstifadəçinin rolu.
        /// Məsələn: Admin, Teacher, Student.
        /// </summary>
        public string Role { get; set; } = string.Empty;

        

        /// <summary>
        /// User yaradılan anda aktiv olsun ya yox.
        /// Default olaraq true verilə bilər.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
