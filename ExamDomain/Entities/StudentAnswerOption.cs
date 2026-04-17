using ExamDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class StudentAnswerOption : BaseEntity
    {
        public int StudentAnswerId { get; set; }
        public StudentAnswer StudentAnswer { get; set; } = null!;

        public int ExamOptionId { get; set; }
        public ExamOption ExamOption { get; set; } = null!;
    }
}
