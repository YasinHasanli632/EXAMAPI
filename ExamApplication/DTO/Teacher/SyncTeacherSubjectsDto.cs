using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher
{
    public class SyncTeacherSubjectsDto
    {
        public int TeacherId { get; set; }

        public List<int> SubjectIds { get; set; } = new();
    }
}
