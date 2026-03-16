using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.User
{
    /// <summary>
    /// User siyahısı ekranında göstəriləcək qısa user məlumat modelidir.
    /// </summary>
    public class UserListItemDto
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
        /// İstifadəçinin sistemdəki rolu.
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// İstifadəçinin aktiv olub-olmadığını göstərir.
        /// </summary>
        public bool IsActive { get; set; }

        

        /// <summary>
        /// User-ə bağlı profil varsa, göstəriləcək tam ad.
        /// Student və ya Teacher profilindən götürülə bilər.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// User-in yaradılma tarixi.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
