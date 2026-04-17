using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Attendance
{
    // YENI
    public class SaveAttendanceSessionRecordsDto
    {
        public int SessionId { get; set; }

        public List<UpsertAttendanceRecordDto> Records { get; set; } = new();
    }
}
