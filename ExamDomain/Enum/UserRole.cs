using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Enum
{
    
        public enum UserRole
        {
            Admin = 1, // Sistemin tam idarəetmə hüququ olan istifadəçi (admin panelə giriş)

            Teacher = 2, // İmtahan yaradan və tələbələri idarə edən müəllim

            Student = 3,// İmtahana girən və nəticələrini görən tələbə
            IsSuperAdmin = 4
        }
    }


