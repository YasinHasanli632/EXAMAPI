using ExamDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class SystemSetting : AuditableEntity
    {
        // ÜMUMİ
        public string SystemName { get; set; } = "Exam Management System";
        public string SchoolName { get; set; } = "Məktəb İdarəetmə Sistemi";
        public string AcademicYear { get; set; } = "2025-2026";
        public string DefaultLanguage { get; set; } = "az";

        // İMTAHAN
        public int DefaultExamDurationMinutes { get; set; } = 90;
        public int AccessCodeActivationMinutes { get; set; } = 5;
        public int LateEntryToleranceMinutes { get; set; } = 10;
        public int DefaultPassScore { get; set; } = 51;

        public bool AutoSubmitOnEndTime { get; set; } = true;
        public bool AllowEarlySubmit { get; set; } = true;
        public bool AutoPublishResults { get; set; } = false;
        public bool ShowScoreImmediately { get; set; } = true;
        public bool ShowCorrectAnswersAfterCompletion { get; set; } = false;

        // BİLDİRİŞ
        public bool ExamPublishNotificationEnabled { get; set; } = true;
        public bool ExamRescheduleNotificationEnabled { get; set; } = true;
        public bool ExamStartNotificationEnabled { get; set; } = true;
        public bool ResultNotificationEnabled { get; set; } = true;
        public bool AttendanceNotificationEnabled { get; set; } = true;
        public bool TaskNotificationEnabled { get; set; } = true;

        // TƏHLÜKƏSİZLİK / SESSİYA
        public int AutoSaveIntervalSeconds { get; set; } = 30;
        public int SessionTimeoutMinutes { get; set; } = 20;
        public int MaxAccessCodeAttempts { get; set; } = 5;
        public bool AllowReEntry { get; set; } = true;
    }
}
