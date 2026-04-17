using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class UpdateStudentTaskDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public int? SubjectId { get; set; }

        public int? TeacherId { get; set; }

        public DateTime AssignedDate { get; set; }

        public DateTime DueDate { get; set; }

        public int Status { get; set; }

        public decimal Score { get; set; }

        public decimal MaxScore { get; set; }

        public string? Link { get; set; }

        public string? Note { get; set; }
    }
}
