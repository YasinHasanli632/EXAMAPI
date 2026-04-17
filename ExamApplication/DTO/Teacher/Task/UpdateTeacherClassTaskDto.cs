using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher.Task
{
    public class UpdateTeacherClassTaskDto
    {
        public string TaskGroupKey { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }

        public decimal MaxScore { get; set; }

        public string? Link { get; set; }
        public string? Note { get; set; }
    }
}
