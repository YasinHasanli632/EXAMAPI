using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Class
{
    public class ClassRoomTeacherDto
    {
        // Teacher entity Id-si
        public int TeacherId { get; set; }

        // Teacher-ə bağlı user Id-si
        public int UserId { get; set; }

        // Müəllimin tam adı
        public string FullName { get; set; } = null!;

        // Müəllimin username-i
        public string UserName { get; set; } = null!;

        // Müəllimin email-i
        public string Email { get; set; } = null!;
    }
}
