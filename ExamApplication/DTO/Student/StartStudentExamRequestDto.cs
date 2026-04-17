using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class StartStudentExamRequestDto
    {
        public int ExamId { get; set; }

        // YENI
        public string? AccessCode { get; set; }
        // YENI
        public bool AcceptRules { get; set; }
    }
}
