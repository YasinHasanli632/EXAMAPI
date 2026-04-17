using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Helper
{
    public static class AzerbaijanTimeHelper
    {
        // YENI
        private static readonly TimeZoneInfo BakuTimeZone = ResolveBakuTimeZone();

        // YENI
        public static DateTime GetNow()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, BakuTimeZone);
        }

        // YENI
        public static DateTime ToBakuTime(DateTime value)
        {
            if (value.Kind == DateTimeKind.Utc)
            {
                return TimeZoneInfo.ConvertTimeFromUtc(value, BakuTimeZone);
            }

            if (value.Kind == DateTimeKind.Local)
            {
                return TimeZoneInfo.ConvertTime(value, BakuTimeZone);
            }

            // Unspecified gələn datetime-local dəyərləri olduğu kimi saxlayırıq.
            return value;
        }

        // YENI
        private static TimeZoneInfo ResolveBakuTimeZone()
        {
            var candidates = new[]
            {
                "Asia/Baku",
                "Azerbaijan Standard Time"
            };

            foreach (var id in candidates)
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(id);
                }
                catch
                {
                    // növbəti ID yoxlanır
                }
            }

            return TimeZoneInfo.Local;
        }
    }
}
