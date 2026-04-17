using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher.Task
{
    namespace ExamApplication.DTO.Teacher.Tasks
    {
        public class TeacherClassTaskListItemDto
        {
            public string TaskGroupKey { get; set; } = string.Empty;
            public int ClassRoomId { get; set; }
            public string ClassName { get; set; } = string.Empty;
            public int? SubjectId { get; set; }
            public string SubjectName { get; set; } = string.Empty;
            public int? TeacherId { get; set; }
            public string TeacherName { get; set; } = string.Empty;

            public string Title { get; set; } = string.Empty;
            public string? Description { get; set; }

            public DateTime AssignedDate { get; set; }
            public DateTime DueDate { get; set; }

            public decimal MaxScore { get; set; }

            // Gözləyir / Davam edir / Bitib
            public string TaskState { get; set; } = string.Empty;

            public int TotalStudentCount { get; set; }
            public int SubmittedCount { get; set; }
            public int MissingCount { get; set; }
            public int ReviewedCount { get; set; }
        }
    }
}
