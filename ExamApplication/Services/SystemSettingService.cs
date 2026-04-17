using ExamApplication.DTO.Settings;
using ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Services;
using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Services
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public SystemSettingService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<SystemSettingDto> GetAsync(CancellationToken cancellationToken = default)
        {
            var entity = await GetOrCreateInternalAsync(cancellationToken);
            return MapToDto(entity);
        }

        public async Task<SystemSettingDto> UpdateAsync(
            UpdateSystemSettingDto request,
            CancellationToken cancellationToken = default)
        {
            var entity = await GetOrCreateInternalAsync(cancellationToken);

            ApplyUpdate(entity, request);

            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedByUserId = _currentUserService.GetCurrentUser()?.UserId;

            _unitOfWork.SystemSettings.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(entity);
        }

        public async Task<SystemSettingDto> ResetToDefaultsAsync(CancellationToken cancellationToken = default)
        {
            var entity = await GetOrCreateInternalAsync(cancellationToken);

            ApplyDefaults(entity);

            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedByUserId = _currentUserService.GetCurrentUser()?.UserId;

            _unitOfWork.SystemSettings.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(entity);
        }

        private async Task<SystemSetting> GetOrCreateInternalAsync(CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.SystemSettings.GetSingleAsync(cancellationToken);
            if (entity != null)
            {
                return entity;
            }

            entity = CreateDefaultEntity();
            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedByUserId = _currentUserService.GetCurrentUser()?.UserId;

            await _unitOfWork.SystemSettings.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return entity;
        }

        private static SystemSetting CreateDefaultEntity()
        {
            return new SystemSetting
            {
                SystemName = "Exam Management System",
                SchoolName = "Məktəb İdarəetmə Sistemi",
                AcademicYear = "2025-2026",
                DefaultLanguage = "az",

                DefaultExamDurationMinutes = 90,
                AccessCodeActivationMinutes = 5,
                LateEntryToleranceMinutes = 10,
                DefaultPassScore = 51,

                AutoSubmitOnEndTime = true,
                AllowEarlySubmit = true,
                AutoPublishResults = false,
                ShowScoreImmediately = true,
                ShowCorrectAnswersAfterCompletion = false,

                ExamPublishNotificationEnabled = true,
                ExamRescheduleNotificationEnabled = true,
                ExamStartNotificationEnabled = true,
                ResultNotificationEnabled = true,
                AttendanceNotificationEnabled = true,
                TaskNotificationEnabled = true,

                AutoSaveIntervalSeconds = 30,
                SessionTimeoutMinutes = 20,
                MaxAccessCodeAttempts = 5,
                AllowReEntry = true
            };
        }

        private static void ApplyDefaults(SystemSetting entity)
        {
            var defaults = CreateDefaultEntity();

            entity.SystemName = defaults.SystemName;
            entity.SchoolName = defaults.SchoolName;
            entity.AcademicYear = defaults.AcademicYear;
            entity.DefaultLanguage = defaults.DefaultLanguage;

            entity.DefaultExamDurationMinutes = defaults.DefaultExamDurationMinutes;
            entity.AccessCodeActivationMinutes = defaults.AccessCodeActivationMinutes;
            entity.LateEntryToleranceMinutes = defaults.LateEntryToleranceMinutes;
            entity.DefaultPassScore = defaults.DefaultPassScore;

            entity.AutoSubmitOnEndTime = defaults.AutoSubmitOnEndTime;
            entity.AllowEarlySubmit = defaults.AllowEarlySubmit;
            entity.AutoPublishResults = defaults.AutoPublishResults;
            entity.ShowScoreImmediately = defaults.ShowScoreImmediately;
            entity.ShowCorrectAnswersAfterCompletion = defaults.ShowCorrectAnswersAfterCompletion;

            entity.ExamPublishNotificationEnabled = defaults.ExamPublishNotificationEnabled;
            entity.ExamRescheduleNotificationEnabled = defaults.ExamRescheduleNotificationEnabled;
            entity.ExamStartNotificationEnabled = defaults.ExamStartNotificationEnabled;
            entity.ResultNotificationEnabled = defaults.ResultNotificationEnabled;
            entity.AttendanceNotificationEnabled = defaults.AttendanceNotificationEnabled;
            entity.TaskNotificationEnabled = defaults.TaskNotificationEnabled;

            entity.AutoSaveIntervalSeconds = defaults.AutoSaveIntervalSeconds;
            entity.SessionTimeoutMinutes = defaults.SessionTimeoutMinutes;
            entity.MaxAccessCodeAttempts = defaults.MaxAccessCodeAttempts;
            entity.AllowReEntry = defaults.AllowReEntry;
        }

        private static void ApplyUpdate(SystemSetting entity, UpdateSystemSettingDto request)
        {
            entity.SystemName = NormalizeText(request.SystemName, "Exam Management System", 200);
            entity.SchoolName = NormalizeText(request.SchoolName, "Məktəb İdarəetmə Sistemi", 200);
            entity.AcademicYear = NormalizeText(request.AcademicYear, "2025-2026", 30);
            entity.DefaultLanguage = NormalizeText(request.DefaultLanguage, "az", 20);

            entity.DefaultExamDurationMinutes = request.DefaultExamDurationMinutes > 0
                ? request.DefaultExamDurationMinutes
                : 90;

            entity.AccessCodeActivationMinutes = request.AccessCodeActivationMinutes >= 0
                ? request.AccessCodeActivationMinutes
                : 5;

            entity.LateEntryToleranceMinutes = request.LateEntryToleranceMinutes >= 0
                ? request.LateEntryToleranceMinutes
                : 10;

            entity.DefaultPassScore = request.DefaultPassScore >= 0 && request.DefaultPassScore <= 100
                ? request.DefaultPassScore
                : 51;

            entity.AutoSubmitOnEndTime = request.AutoSubmitOnEndTime;
            entity.AllowEarlySubmit = request.AllowEarlySubmit;
            entity.AutoPublishResults = request.AutoPublishResults;
            entity.ShowScoreImmediately = request.ShowScoreImmediately;
            entity.ShowCorrectAnswersAfterCompletion = request.ShowCorrectAnswersAfterCompletion;

            entity.ExamPublishNotificationEnabled = request.ExamPublishNotificationEnabled;
            entity.ExamRescheduleNotificationEnabled = request.ExamRescheduleNotificationEnabled;
            entity.ExamStartNotificationEnabled = request.ExamStartNotificationEnabled;
            entity.ResultNotificationEnabled = request.ResultNotificationEnabled;
            entity.AttendanceNotificationEnabled = request.AttendanceNotificationEnabled;
            entity.TaskNotificationEnabled = request.TaskNotificationEnabled;

            entity.AutoSaveIntervalSeconds = request.AutoSaveIntervalSeconds > 0
                ? request.AutoSaveIntervalSeconds
                : 30;

            entity.SessionTimeoutMinutes = request.SessionTimeoutMinutes > 0
                ? request.SessionTimeoutMinutes
                : 20;

            entity.MaxAccessCodeAttempts = request.MaxAccessCodeAttempts > 0
                ? request.MaxAccessCodeAttempts
                : 5;

            entity.AllowReEntry = request.AllowReEntry;
        }

        private static string NormalizeText(string? value, string fallback, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return fallback;
            }

            var trimmed = value.Trim();
            return trimmed.Length <= maxLength
                ? trimmed
                : trimmed[..maxLength];
        }

        private static SystemSettingDto MapToDto(SystemSetting entity)
        {
            return new SystemSettingDto
            {
                Id = entity.Id,

                SystemName = entity.SystemName,
                SchoolName = entity.SchoolName,
                AcademicYear = entity.AcademicYear,
                DefaultLanguage = entity.DefaultLanguage,

                DefaultExamDurationMinutes = entity.DefaultExamDurationMinutes,
                AccessCodeActivationMinutes = entity.AccessCodeActivationMinutes,
                LateEntryToleranceMinutes = entity.LateEntryToleranceMinutes,
                DefaultPassScore = entity.DefaultPassScore,

                AutoSubmitOnEndTime = entity.AutoSubmitOnEndTime,
                AllowEarlySubmit = entity.AllowEarlySubmit,
                AutoPublishResults = entity.AutoPublishResults,
                ShowScoreImmediately = entity.ShowScoreImmediately,
                ShowCorrectAnswersAfterCompletion = entity.ShowCorrectAnswersAfterCompletion,

                ExamPublishNotificationEnabled = entity.ExamPublishNotificationEnabled,
                ExamRescheduleNotificationEnabled = entity.ExamRescheduleNotificationEnabled,
                ExamStartNotificationEnabled = entity.ExamStartNotificationEnabled,
                ResultNotificationEnabled = entity.ResultNotificationEnabled,
                AttendanceNotificationEnabled = entity.AttendanceNotificationEnabled,
                TaskNotificationEnabled = entity.TaskNotificationEnabled,

                AutoSaveIntervalSeconds = entity.AutoSaveIntervalSeconds,
                SessionTimeoutMinutes = entity.SessionTimeoutMinutes,
                MaxAccessCodeAttempts = entity.MaxAccessCodeAttempts,
                AllowReEntry = entity.AllowReEntry,

                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}
