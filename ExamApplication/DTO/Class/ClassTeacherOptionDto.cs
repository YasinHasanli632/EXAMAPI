using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Class
{
    public class ClassTeacherOptionDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = string.Empty;

        public string PhotoUrl { get; set; } = string.Empty;

        public List<int> SubjectIds { get; set; } = new();

        public List<string> SubjectNames { get; set; } = new();

        public string Status { get; set; } = "Aktiv";
    }
}
