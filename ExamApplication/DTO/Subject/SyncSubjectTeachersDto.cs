using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Subject
{
    public class SyncSubjectTeachersDto
    {
        public int SubjectId { get; set; }

        public List<int> TeacherIds { get; set; } = new();
    }
}
