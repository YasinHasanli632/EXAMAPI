using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Auth
{
    /// <summary>
    /// Hal-hazırda login olmuş istifadəçinin əsas məlumatlarını qaytarır.
    /// Bu model frontend-də header, profil və role-based yönləndirmə üçün istifadə olunur.
    /// </summary>
    public class CurrentUserDto
    {
        /// <summary>
        /// İstifadəçinin sistemdəki unikal Id-si.
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
        /// Məsələn: Admin, Teacher, Student.
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// İstifadəçinin aktiv olub-olmadığını göstərir.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// İstifadəçinin ekranda göstəriləcək tam adı.
        /// Student və ya Teacher profilindən götürülə bilər.
        /// </summary>
        public string FullName { get; set; } = string.Empty;
    }
}
