using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher
{
    public class SyncTeacherClassRoomsDto
    {
        public int TeacherId { get; set; }

        public List<TeacherClassRoomAssignmentDto> Assignments { get; set; } = new();
    }
}
