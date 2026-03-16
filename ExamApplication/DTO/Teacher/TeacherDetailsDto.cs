using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher
{
    public class TeacherDetailsDto
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

        // Müəllimin fənnləri
        public List<TeacherSubjectDto> Subjects { get; set; } = new();

        // Müəllimin sinifləri
        public List<TeacherClassRoomDto> ClassRooms { get; set; } = new();

        // Müəllimin yaratdığı imtahan sayı
        public int ExamCount { get; set; }
    }
}
