using ExamDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class TeacherClassRoom : BaseEntity
    {
        public int TeacherId { get; set; } // Müəllimin Id-si

        public int ClassRoomId { get; set; } // Sinfin Id-si

        public Teacher Teacher { get; set; } = null!; // Əlaqəli müəllim

        public ClassRoom ClassRoom { get; set; } = null!; // Əlaqəli sinif
    }
}
