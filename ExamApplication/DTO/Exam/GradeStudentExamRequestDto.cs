using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Exam
{
    public class GradeStudentExamRequestDto
    {
        public int StudentExamId { get; set; }
        public List<GradeStudentAnswerRequestDto> Answers { get; set; } = new();
    }
}
