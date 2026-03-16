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
    }
}
