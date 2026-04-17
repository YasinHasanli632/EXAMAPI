using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class UpdateStudentDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public DateTime DateOfBirth { get; set; }

        public string StudentNumber { get; set; } = null!;

        public int? ClassRoomId { get; set; }

        public int? Status { get; set; }

        public string? Notes { get; set; }
    }
}
