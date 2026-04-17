using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Settings
{
    public class SystemSettingDto
    {
        public int Id { get; set; }

        // ÜMUMİ
        public string SystemName { get; set; } = string.Empty;
        public string SchoolName { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public string DefaultLanguage { get; set; } = string.Empty;

        // İMTAHAN
        public int DefaultExamDurationMinutes { get; set; }
        public int AccessCodeActivationMinutes { get; set; }
        public int LateEntryToleranceMinutes { get; set; }
        public int DefaultPassScore { get; set; }

        public bool AutoSubmitOnEndTime { get; set; }
        public bool AllowEarlySubmit { get; set; }
        public bool AutoPublishResults { get; set; }
        public bool ShowScoreImmediately { get; set; }
        public bool ShowCorrectAnswersAfterCompletion { get; set; }

        // BİLDİRİŞ
        public bool ExamPublishNotificationEnabled { get; set; }
        public bool ExamRescheduleNotificationEnabled { get; set; }
        public bool ExamStartNotificationEnabled { get; set; }
        public bool ResultNotificationEnabled { get; set; }
        public bool AttendanceNotificationEnabled { get; set; }
        public bool TaskNotificationEnabled { get; set; }

        // TƏHLÜKƏSİZLİK / SESSİYA
        public int AutoSaveIntervalSeconds { get; set; }
        public int SessionTimeoutMinutes { get; set; }
        public int MaxAccessCodeAttempts { get; set; }
        public bool AllowReEntry { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
