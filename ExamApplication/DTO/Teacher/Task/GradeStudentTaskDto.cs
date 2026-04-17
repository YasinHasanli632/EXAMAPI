using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher.Task
{
    public class GradeStudentTaskDto
    {
        public int StudentTaskId { get; set; }
        public decimal Score { get; set; }
        public string? Feedback { get; set; }
    }
}
