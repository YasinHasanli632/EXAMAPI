using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    // Student-in cavab save etmə request modelidir.
    public class SaveStudentAnswerRequestDto
    {
        // Aid olduğu exam session Id-si.
        public int StudentExamId { get; set; }

        // Aid olduğu sual Id-si.
        public int ExamQuestionId { get; set; }

        // Test sual üçün seçilən variant Id-si.
        public int? SelectedOptionId { get; set; }

        // Açıq sual üçün yazılan cavab mətni.
        public string? AnswerText { get; set; }
    }
}
