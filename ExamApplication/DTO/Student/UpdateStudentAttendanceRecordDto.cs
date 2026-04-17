using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class UpdateStudentAttendanceRecordDto
    {
        public int AttendanceSessionId { get; set; }

        // Present = 1, Absent = 2, Late = 3
        public int Status { get; set; }

        public string? Note { get; set; }
    }
}
