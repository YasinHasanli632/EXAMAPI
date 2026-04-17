using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.User
{
    public class CreateUserRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // YENİ
        public string FatherName { get; set; } = string.Empty;

        public DateTime? BirthDate { get; set; }
        public string? PhoneNumber { get; set; }

        // YENİ
        public string? Country { get; set; }

        public string? PhotoUrl { get; set; }
        public string? Details { get; set; }
    }
}
