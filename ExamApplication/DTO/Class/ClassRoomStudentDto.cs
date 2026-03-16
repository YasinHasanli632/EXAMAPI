using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Class
{
    public class ClassRoomStudentDto
    {
        // Student entity Id-si
        public int StudentId { get; set; }

        // Student-ə bağlı user Id-si
        public int UserId { get; set; }

        // Tələbənin tam adı
        public string FullName { get; set; } = null!;

        // Tələbənin username-i
        public string UserName { get; set; } = null!;

        // Tələbənin email-i
        public string Email { get; set; } = null!;
    }
}
