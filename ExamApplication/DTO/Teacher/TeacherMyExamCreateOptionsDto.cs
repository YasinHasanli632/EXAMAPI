using ExamApplication.DTO.Exam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher
{
    public class TeacherMyExamCreateOptionsDto
    {
        public int TeacherId { get; set; }

        public string TeacherName { get; set; } = null!;

        public List<ExamClassOptionDto> ClassOptions { get; set; } = new();

        public List<ExamSubjectOptionDto> SubjectOptions { get; set; } = new();
    }
}
