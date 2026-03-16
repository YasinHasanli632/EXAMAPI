using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.ValueObjects
{
    public class ExamTimeRange
    {
        public DateTime StartTime { get; private set; } // Başlama vaxtı
        public DateTime EndTime { get; private set; } // Bitmə vaxtı

        public ExamTimeRange(DateTime startTime, DateTime endTime)
        {
            if (endTime <= startTime)
                throw new ArgumentException("Bitmə vaxtı başlama vaxtından böyük olmalıdır.");

            StartTime = startTime;
            EndTime = endTime;
        }

        public int DurationMinutes => (int)(EndTime - StartTime).TotalMinutes; // Müddət
    }
}
