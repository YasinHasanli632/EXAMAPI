using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher
{
    public class TeacherOverviewStatsDto
    {
        public int SubjectCount { get; set; }

        public int ClassRoomCount { get; set; }

        public int StudentCount { get; set; }

        public int ExamCount { get; set; }

        public int PendingTaskCount { get; set; }

        public int CompletedTaskCount { get; set; }
    }
}
