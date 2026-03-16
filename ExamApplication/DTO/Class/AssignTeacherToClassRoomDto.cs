using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Class
{
    public class AssignTeacherToClassRoomDto
    {
        // Müəllimin Id-si
        public int TeacherId { get; set; }

        // Sinifin Id-si
        public int ClassRoomId { get; set; }
    }
}
