using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Enum
{
    public enum ExamStatus
    {
        Draft = 1, // İmtahan yaradılıb, amma hələ publish olunmayıb

        Planned = 2, // İmtahan publish olunub, amma başlama vaxtı hələ gəlməyib

        Active = 3, // İmtahan hal-hazırda aktivdir və tələbələr daxil ola bilirlər

        Completed = 4, // İmtahanın vaxtı bitib və imtahan tamamlanıb

        Cancelled = 5 // İmtahan ləğv edilib
    }
}
