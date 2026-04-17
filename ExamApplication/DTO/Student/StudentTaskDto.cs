using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class StudentTaskDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string SubjectName { get; set; } = string.Empty;

        public string TeacherName { get; set; } = string.Empty;

        public DateTime AssignedDate { get; set; }

        public DateTime DueDate { get; set; }

        public string Status { get; set; } = null!;

        public decimal Score { get; set; }

        public decimal MaxScore { get; set; }

        public string? Link { get; set; }

        public string? Note { get; set; }

        // YENI
        public string? Description { get; set; }
    }
}
