using ExamDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class ExamOption : BaseEntity
    {
        public int ExamQuestionId { get; set; } // Bu variantın aid olduğu sual

        public string OptionText { get; set; } = null!; // Variantın mətni

        public bool IsCorrect { get; set; } // Bu variant doğru cavabdır ya yox

        public ExamQuestion ExamQuestion { get; set; } = null!; // Navigation property

        // YENI
        // Frontenddə A/B/C/D label rahatlığı üçün
        public string? OptionKey { get; set; }

        // YENI
        public int OrderNumber { get; set; }
    }
}
