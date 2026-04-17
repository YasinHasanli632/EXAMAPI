using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class CreateStudentTaskDto
    {
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public int? SubjectId { get; set; }

        public int? TeacherId { get; set; }

        public DateTime AssignedDate { get; set; }

        public DateTime DueDate { get; set; }

        public int Status { get; set; } = 1;

        public decimal Score { get; set; } = 0;

        public decimal MaxScore { get; set; } = 100;

        public string? Link { get; set; }

        public string? Note { get; set; }
    }
}
