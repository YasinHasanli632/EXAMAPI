using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher.Task
{
    public class TeacherTaskClassSummaryDto
    {
        public int ClassRoomId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public string? Room { get; set; }
        public int StudentCount { get; set; }

        // YENI
        public List<TeacherTaskClassSubjectDto> Subjects { get; set; } = new();

        // YENI
        public int TotalTaskCount { get; set; }
        public int ActiveTaskCount { get; set; }
        public int CompletedTaskCount { get; set; }
        public int PendingReviewCount { get; set; }
    }
    public class TeacherTaskClassSubjectDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
    }

}
