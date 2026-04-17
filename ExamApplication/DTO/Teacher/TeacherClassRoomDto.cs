using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher
{
    public class TeacherClassRoomDto
    {
        // Əlaqənin Id-si
        public int Id { get; set; }

        // Sinfin Id-si
        public int ClassRoomId { get; set; }

        // Sinfin adı
        public string ClassRoomName { get; set; } = null!;

        // Sinfin səviyyəsi
        public int Grade { get; set; }

        // YENI
        public int? SubjectId { get; set; }

        // YENI
        public string? SubjectName { get; set; }

        // YENI
        public bool IsActive { get; set; }
    }
}
