using ExamDomain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher
{
    public class ChangeTeacherStatusDto
    {
        public int TeacherId { get; set; }

        public TeacherStatus Status { get; set; }
    }
}
