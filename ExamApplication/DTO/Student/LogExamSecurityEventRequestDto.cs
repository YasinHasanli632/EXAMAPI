using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class LogExamSecurityEventRequestDto
    {
        public int StudentExamId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
