using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    // Student-in exam başlatma request modelidir.
    public class StartStudentExamRequestDto
    {
        // Sistemdə login olmuş user Id-si.
        public int UserId { get; set; }

        // Başlanacaq exam Id-si.
        public int ExamId { get; set; }
    }
}
