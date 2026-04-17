using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Exam
{
    public class TeacherExamListFilterDto
    {
        public int? ClassRoomId { get; set; }

        public int? SubjectId { get; set; }

        public bool? IsPublished { get; set; }

        public string? Status { get; set; }
    }
}
