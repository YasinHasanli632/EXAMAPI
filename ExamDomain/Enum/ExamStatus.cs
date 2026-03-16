using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Enum
{
    public enum ExamStatus
    {
        Draft = 1, // İmtahan yaradılıb amma hələ aktiv edilməyib

        Published = 2, // İmtahan yayımlanıb və tələbələr üçün görünür

        Active = 3, // Hal-hazırda keçirilən aktiv imtahan

        Completed = 4, // İmtahan tamamlanıb və bağlanıb

        Cancelled = 5 // İmtahan ləğv edilib
    }
}
