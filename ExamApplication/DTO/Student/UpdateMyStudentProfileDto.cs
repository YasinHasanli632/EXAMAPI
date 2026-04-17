using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class UpdateMyStudentProfileDto
    {
        // YENI
        public string FullName { get; set; } = string.Empty;

        // YENI
        public string FirstName { get; set; } = string.Empty;

        // YENI
        public string LastName { get; set; } = string.Empty;

        // YENI
        public string FatherName { get; set; } = string.Empty;

        // YENI
        public string Email { get; set; } = string.Empty;

        // YENI
        public string? PhoneNumber { get; set; }

        // YENI
        public string? ParentPhone { get; set; }

        // YENI
        public string? Address { get; set; }

        // YENI
        public string Gender { get; set; } = "Bilinmir";

        // YENI
        public string StudentNumber { get; set; } = string.Empty;

        // YENI
        public DateTime DateOfBirth { get; set; }

        // YENI
        public string Status { get; set; } = "Aktiv";

        // YENI
        public string? Notes { get; set; }

        // YENI
        public string? PhotoUrl { get; set; }
    }
}
