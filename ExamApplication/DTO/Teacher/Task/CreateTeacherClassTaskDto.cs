using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher.Task
{
    public class CreateTeacherClassTaskDto
    {
        public int ClassRoomId { get; set; }
        public int SubjectId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }

        public decimal MaxScore { get; set; } = 100;

        public string? Link { get; set; }
        public string? Note { get; set; }
    }
}
