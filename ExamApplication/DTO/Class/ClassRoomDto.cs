using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Class
{
    public class ClassRoomDto
    {
        // Sinifin Id-si
        public int Id { get; set; }

        // Sinif adı
        public string Name { get; set; } = null!;

        // Sinif səviyyəsi
        public int Grade { get; set; }
    }
}
