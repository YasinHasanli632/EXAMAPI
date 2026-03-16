using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    // Student profil məlumatlarını daşıyır.
    public class StudentProfileDto
    {
        // Student entity Id-si.
        public int StudentId { get; set; }

        // Bağlı olduğu user hesabının Id-si.
        public int UserId { get; set; }

        // Tələbənin tam adı.
        public string FullName { get; set; } = null!;

        // Tələbənin doğum tarixi.
        public DateTime DateOfBirth { get; set; }

        // Məktəb daxili tələbə nömrəsi.
        public string StudentNumber { get; set; } = null!;

        // User username dəyəri.
        public string Username { get; set; } = null!;

        // User email dəyəri.
        public string Email { get; set; } = null!;
    }
}
