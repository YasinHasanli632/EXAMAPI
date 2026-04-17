using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class SubmitStudentExamRequestDto
    {
        public int StudentExamId { get; set; }
        // YENI
        public bool ForceAutoSubmit { get; set; }
    }
}
