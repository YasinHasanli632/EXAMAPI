using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher
{
    public class TeacherDto
    {
        // Müəllimin Id-si
        public int Id { get; set; }

        // Müəllimin bağlı olduğu user id
        public int UserId { get; set; }

        // Müəllimin tam adı
        public string FullName { get; set; } = null!;

        // Müəllimin bölməsi
        public string Department { get; set; } = null!;

        // Username
        public string UserName { get; set; } = null!;

        // Email
        public string Email { get; set; } = null!;

        // YENI
        public string FirstName { get; set; } = string.Empty;

        // YENI
        public string LastName { get; set; } = string.Empty;

        // YENI
        public string FatherName { get; set; } = string.Empty;

        // YENI
        public string? PhoneNumber { get; set; }

        // YENI
        public string? PhotoUrl { get; set; }

        // YENI
        public string? Country { get; set; }

        // YENI
        public DateTime? BirthDate { get; set; }

        // YENI
        public string? Details { get; set; }

        // YENI
        public string? Specialization { get; set; }

        // YENI
        public string? Status { get; set; }

        // YENI
        public bool IsActive { get; set; }



    }
}
