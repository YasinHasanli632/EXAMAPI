using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.User
{
    public class UserListItemDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // YENİ
        public string FatherName { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        public int? Age { get; set; }
        public string? PhoneNumber { get; set; }

        // YENİ
        public string? Country { get; set; }

        public string? PhotoUrl { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
