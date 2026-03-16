using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher
{
    public class RemoveSubjectFromTeacherDto
    {
        // Müəllimin Id-si
        public int TeacherId { get; set; }

        // Fənnin Id-si
        public int SubjectId { get; set; }
    }
}
