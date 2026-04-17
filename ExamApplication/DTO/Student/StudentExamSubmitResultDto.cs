using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class StudentExamSubmitResultDto
    {
        public int StudentExamId { get; set; }
        public int ExamId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsCompleted { get; set; }
        public decimal Score { get; set; }
        public string Message { get; set; } = string.Empty;

        // YENI
        // Qapalı suallar üzrə avtomatik hesablanan bal
        public decimal AutoGradedScore { get; set; }

        // YENI
        // Açıq suallar üçün manual hissə
        public decimal ManualGradedScore { get; set; }

        // YENI
        // Açıq sual varsa müəllim yoxlaması tələb olunur
        public bool RequiresManualReview { get; set; }

        // YENI
        // İmtahanda açıq sual olub-olmadığını frontend bilsin
        public bool HasOpenQuestions { get; set; }

        // YENI
        // Result auto publish oluna bilərmi
        public bool IsResultAutoPublished { get; set; }

        // YENI
        // Submitdən dərhal sonra score göstərmək olarmı
        public bool CanShowScoreImmediately { get; set; }

        // YENI
        // Correct answers göstərmək olarmı
        public bool CanShowCorrectAnswers { get; set; }

        // YENI
        // Frontend əsas bunu göstərsin.
        // null-dursa score UI-də göstərilməsin
        public decimal? PublishedScore { get; set; }
    }
}
