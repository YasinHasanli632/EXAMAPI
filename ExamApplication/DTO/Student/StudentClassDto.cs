using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    // Student-in qoşulduğu sinif məlumatını daşıyır.
    public class StudentClassDto
    {
        // StudentClass əlaqə Id-si.
        public int StudentClassId { get; set; }

        // Sinfin Id-si.
        public int ClassRoomId { get; set; }

        // Sinfin adı.
        public string ClassName { get; set; } = null!;

        // Sinfin grade dəyəri.
        public int Grade { get; set; }
    }
}
