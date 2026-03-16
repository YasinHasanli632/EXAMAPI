using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Class
{
    public class ClassRoomDetailsDto
    {
        // Sinifin Id-si
        public int Id { get; set; }

        // Sinif adı
        public string Name { get; set; } = null!;

        // Sinif səviyyəsi
        public int Grade { get; set; }

        // Sinifdə olan tələbələrin sayı
        public int StudentCount { get; set; }

        // Sinifə bağlı müəllimlərin sayı
        public int TeacherCount { get; set; }

        // Sinifdə olan tələbələrin siyahısı
        public List<ClassRoomStudentDto> Students { get; set; } = new();

        // Sinifə bağlı müəllimlərin siyahısı
        public List<ClassRoomTeacherDto> Teachers { get; set; } = new();
    }
}
