using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Helper
{
    public static class AzerbaijanTimeHelper
    {
        private static readonly TimeZoneInfo BakuTimeZone = ResolveBakuTimeZone();

        public static DateTime UtcNow => DateTime.UtcNow;

        public static DateTime BakuNow =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, BakuTimeZone);

        public static DateTime GetBakuToday()
        {
            return BakuNow.Date;
        }

        public static DateTime FromBakuToUtc(DateTime value)
        {
            if (value == default)
                return value;

            if (value.Kind == DateTimeKind.Utc)
                return value;

            if (value.Kind == DateTimeKind.Local)
                return value.ToUniversalTime();

            var unspecified = DateTime.SpecifyKind(value, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(unspecified, BakuTimeZone);
        }

        public static DateTime? FromBakuToUtc(DateTime? value)
        {
            if (!value.HasValue)
                return null;

            return FromBakuToUtc(value.Value);
        }

        public static DateTime ToBakuTime(DateTime value)
        {
            if (value == default)
                return value;

            if (value.Kind == DateTimeKind.Unspecified)
            {
                value = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }

            if (value.Kind == DateTimeKind.Local)
            {
                value = value.ToUniversalTime();
            }

            return TimeZoneInfo.ConvertTimeFromUtc(value, BakuTimeZone);
        }

        public static DateTime? ToBakuTime(DateTime? value)
        {
            if (!value.HasValue)
                return null;

            return ToBakuTime(value.Value);
        }

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
                }
            }

            throw new InvalidOperationException("Bakı vaxt zonası tapılmadı.");
        }
    }
}
