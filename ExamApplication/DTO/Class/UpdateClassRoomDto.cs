using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Class
{
    public class UpdateClassRoomDto
    {
        // Yenilənəcək sinifin Id-si
        public int Id { get; set; }

        // Yeni sinif adı
        public string Name { get; set; } = null!;

        // Yeni sinif səviyyəsi
        public int Grade { get; set; }
    }
}
