using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class VerifyStudentExamAccessCodeRequestDto
    {
        public int ExamId { get; set; }
        public string AccessCode { get; set; } = string.Empty;
    }
}
