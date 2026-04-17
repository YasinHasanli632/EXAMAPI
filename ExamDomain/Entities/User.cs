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

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateTime? BirthDate { get; set; }

        public string? PhoneNumber { get; set; }

        public string? PhotoUrl { get; set; }

        public string? Details { get; set; }

        public string FatherName { get; set; } = string.Empty;

        public string? Country { get; set; }

        // YENI
        public Gender Gender { get; set; } = Gender.Unknown;

        // YENI
        public string? ParentPhone { get; set; }

        // YENI
        public string? Address { get; set; }

        // YENI
        public string FullName => $"{FirstName} {LastName}".Trim();

        // YENI
        public int? Age
        {
            get
            {
                if (!BirthDate.HasValue)
                    return null;

                var today = DateTime.UtcNow.Date;
                var age = today.Year - BirthDate.Value.Year;

                if (BirthDate.Value.Date > today.AddYears(-age))
                    age--;

                return age;
            }
        }

        // YENI - ilk girişdə şifrə dəyişmə üçün
        public bool MustChangePassword { get; set; } = true;

        // YENI - forgot password OTP üçün
        public string? PasswordResetOtp { get; set; }
        public DateTime? PasswordResetOtpExpiresAt { get; set; }
        public bool PasswordResetOtpUsed { get; set; } = false;
        public int PasswordResetOtpAttemptCount { get; set; } = 0;
        public DateTime? PasswordResetRequestedAt { get; set; }

        // YENI
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
