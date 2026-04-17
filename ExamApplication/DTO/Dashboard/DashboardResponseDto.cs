using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Dashboard
{
    public class DashboardResponseDto
    {
        public string Role { get; set; } = string.Empty;

        public AdminDashboardDto? Admin { get; set; }

        public TeacherDashboardDto? Teacher { get; set; }

        public StudentDashboardDto? Student { get; set; }
    }
}
