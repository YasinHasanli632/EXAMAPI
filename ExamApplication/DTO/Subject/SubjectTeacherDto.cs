using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Subject
{
    public class SubjectTeacherDto
    {
        public int TeacherId { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhotoUrl { get; set; }

        public string? Status { get; set; }

        public bool IsActive { get; set; }
    }
}
