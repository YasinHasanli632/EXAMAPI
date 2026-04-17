using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Exam
{
    public class UpdateExamDto : CreateExamDto
    {
        public int Id { get; set; }
    }
}
