using ExamDomain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    // Exam daxilində studentə göstərilən sual məlumatını daşıyır.
    public class StudentExamQuestionDto
    {
        // ExamQuestion Id-si.
        public int ExamQuestionId { get; set; }

        // Sual mətni.
        public string QuestionText { get; set; } = null!;

        // Sual tipi.
        public QuestionType QuestionType { get; set; }

        // Sualın balı.
        public decimal Points { get; set; }

        // Sualın imtahandakı sırası.
        public int OrderNumber { get; set; }

        // Əgər test sualdırsa seçilmiş variant.
        public int? SelectedOptionId { get; set; }

        // Əgər açıq sualdırsa yazılan cavab.
        public string? AnswerText { get; set; }

        // Variant siyahısı.
        public List<StudentQuestionOptionDto> Options { get; set; } = new();
    }
}
