using ExamDomain.Common;
using ExamDomain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class User : AuditableEntity
    {
        public string Username { get; set; } = null!; // Login üçün istifadə olunan istifadəçi adı

        public string PasswordHash { get; set; } = null!; // Şifrənin hash olunmuş forması

        public string Email { get; set; } = null!; // İstifadəçinin email ünvanı

        public UserRole Role { get; set; } // İstifadəçinin rolu (Admin, Teacher, Student)

        public bool IsActive { get; set; } // Hesab aktivdir ya yox
        public Student? Student { get; set; } // Əgər user tələbədirsə Student profili

        public Teacher? Teacher { get; set; } // Əgər user müəllimdirsə Teacher profili

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>(); // İstifadəçiyə göndərilən bildirişlər
    }
}
