using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Exam
{
    public class ExamOptionDto
    {
        public int Id { get; set; }

        public string OptionText { get; set; } = null!;

        public bool IsCorrect { get; set; }

        public string? OptionKey { get; set; }

        public int OrderNumber { get; set; }
    }
}
