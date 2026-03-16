using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.User
{
    /// <summary>
    /// User detail səhifəsində göstəriləcək geniş user məlumat modelidir.
    /// </summary>
    public class UserDetailDto
    {
        /// <summary>
        /// User-in unikal Id-si.
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
        /// İstifadəçinin rolu.
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// İstifadəçinin aktiv olub-olmadığını göstərir.
        /// </summary>
        public bool IsActive { get; set; }

       

        /// <summary>
        /// User-ə bağlı profil varsa tam adı.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Əgər user teacher profilinə bağlıdırsa, teacher profil Id-si.
        /// </summary>
        public int? TeacherId { get; set; }

        /// <summary>
        /// Əgər user student profilinə bağlıdırsa, student profil Id-si.
        /// </summary>
        public int? StudentId { get; set; }

        /// <summary>
        /// User-in yaradılma tarixi.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// User-in son yenilənmə tarixi.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
