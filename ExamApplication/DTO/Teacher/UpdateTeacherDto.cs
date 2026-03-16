using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher
{
    public class UpdateTeacherDto
    {
        // Yenilənəcək müəllimin Id-si
        public int Id { get; set; }

        // Müəllimin tam adı
        public string FullName { get; set; } = null!;

        // Müəllimin bölməsi
        public string Department { get; set; } = null!;
    }
}