using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher
{
    public class RemoveClassRoomFromTeacherDto
    {
        // Müəllimin Id-si
        public int TeacherId { get; set; }

        // Sinfin Id-si
        public int ClassRoomId { get; set; }

        // YENI
        public int SubjectId { get; set; }
    }
}
