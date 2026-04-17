using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Attendance
{
    // YENI
    public class AttendanceChangeRequestFilterDto
    {
        public string? RequestStatus { get; set; }

        public int? ClassRoomId { get; set; }
        public int? SubjectId { get; set; }
        public int? TeacherId { get; set; }

        public DateTime? AttendanceDateFrom { get; set; }
        public DateTime? AttendanceDateTo { get; set; }
    }
}
