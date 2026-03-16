using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    // Student-in exam submit etmə request modelidir.
    public class SubmitStudentExamRequestDto
    {
        // Submit ediləcək student exam session Id-si.
        public int StudentExamId { get; set; }
    }
}
