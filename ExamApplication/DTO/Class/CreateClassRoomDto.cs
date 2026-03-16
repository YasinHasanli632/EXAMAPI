using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Class
{
    public class CreateClassRoomDto
    {
        // Sinif adı (məs: 9A, 10B)
        public string Name { get; set; } = null!;

        // Sinif səviyyəsi (məs: 9, 10, 11)
        public int Grade { get; set; }
    }
}
