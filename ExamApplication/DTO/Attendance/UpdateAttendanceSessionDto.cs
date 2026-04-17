using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Attendance
{
    public class UpdateAttendanceSessionDto : CreateAttendanceSessionDto
    {
        public int Id { get; set; }
    }
}
