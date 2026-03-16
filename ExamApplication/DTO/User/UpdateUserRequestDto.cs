using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.User
{
    /// <summary>
    /// Mövcud user-in əsas hesab məlumatlarını yeniləmək üçün istifadə olunan request modelidir.
    /// </summary>
    public class UpdateUserRequestDto
    {
        /// <summary>
        /// Yenilənəcək user-in Id-si.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Yeni username dəyəri.
        /// Sistemdə başqa user tərəfindən istifadə olunmamalıdır.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Yeni email ünvanı.
        /// Sistemdə başqa user tərəfindən istifadə olunmamalıdır.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User-in yeni rolu.
        /// Məsələn: Admin, Teacher, Student.
        /// </summary>
        public string Role { get; set; } = string.Empty;

    
    }
}
