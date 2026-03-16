using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    // Sual variantını daşıyır.
    public class StudentQuestionOptionDto
    {
        // Variant Id-si.
        public int OptionId { get; set; }

        // Variant mətni.
        public string OptionText { get; set; } = null!;

        // Ekranda göstəriləcək sıra nömrəsi.
        public int OrderNumber { get; set; }
    }
}
