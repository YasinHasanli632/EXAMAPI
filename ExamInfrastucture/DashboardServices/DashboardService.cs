using ExamApplication.DTO.Dashboard;
using ExamApplication.Interfaces.Services;
using ExamInfrastucture.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace ExamInfrastucture.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public DashboardService(
            AppDbContext dbContext,
            ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<DashboardResponseDto> GetMyDashboardAsync(CancellationToken cancellationToken = default)
        {
            var currentUser = _currentUserService.GetCurrentUser();
            if (currentUser == null)
                throw new UnauthorizedAccessException("Cari istifadəçi tapılmadı.");

            var role = currentUser.Role?.Trim();

            if (string.IsNullOrWhiteSpace(role))
                throw new UnauthorizedAccessException("İstifadəçi rolu tapılmadı.");

            if (role == "Admin" || role == "IsSuperAdmin")
            {
                var row = await _dbContext.Database
                    .SqlQueryRaw<AdminDashboardRow>("SELECT TOP (1) * FROM dbo.vw_AdminDashboard")
                    .FirstOrDefaultAsync(cancellationToken);

                if (row == null)
                    throw new Exception("Admin dashboard view məlumatı tapılmadı.");

                return new DashboardResponseDto
                {
                    Role = role,
                    Admin = new AdminDashboardDto
                    {
                        ActiveTeacherCount = row.ActiveTeacherCount,
                        TodayAttendanceRate = row.TodayAttendanceRate,
                        TotalClasses = row.TotalClasses,
                        TotalTeachers = row.TotalTeachers,
                        TotalStudents = row.TotalStudents,
                        ActiveExams = row.ActiveExams,
                        TodayExamCount = row.TodayExamCount,
                        TotalSubjects = row.TotalSubjects,
                        TotalAdmins = row.TotalAdmins,
                        Activities = DeserializeJson<List<AdminDashboardActivityDto>>(row.ActivitiesJson) ?? new List<AdminDashboardActivityDto>()
                    }
                };
            }

            if (role == "Teacher")
            {
                var row = await _dbContext.Database
                    .SqlQueryRaw<TeacherDashboardRow>(
                        "SELECT TOP (1) * FROM dbo.vw_TeacherDashboard WHERE UserId = {0}",
                        currentUser.UserId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (row == null)
                    throw new Exception("Teacher dashboard məlumatı tapılmadı.");

                var classes = DeserializeJson<List<TeacherDashboardClassRow>>(row.ClassesJson) ?? new List<TeacherDashboardClassRow>();
                var tasks = DeserializeJson<List<TeacherDashboardTaskItemDto>>(row.RecentTasksJson) ?? new List<TeacherDashboardTaskItemDto>();

                return new DashboardResponseDto
                {
                    Role = role,
                    Teacher = new TeacherDashboardDto
                    {
                        TeacherId = row.TeacherId,
                        UserId = row.UserId,
                        FullName = row.FullName ?? string.Empty,
                        Department = row.Department ?? string.Empty,
                        Specialization = row.Specialization ?? string.Empty,
                        TeacherStatus = row.TeacherStatus,
                        TotalClasses = row.TotalClasses,
                        TotalStudents = row.TotalStudents,
                        TotalExams = row.TotalExams,
                        TotalTasks = row.TotalTasks,
                        PendingTasks = row.PendingTasks,
                        CompletedTasks = row.CompletedTasks,
                        TotalSubjects = row.TotalSubjects,
                        UnreadNotificationsCount = row.UnreadNotificationsCount,
                        Classes = classes.Select(x => new TeacherDashboardClassItemDto
                        {
                            ClassRoomId = x.ClassRoomId,
                            ClassRoomName = x.ClassRoomName ?? string.Empty,
                            StudentCount = x.StudentCount,
                            ExamCount = x.ExamCount,
                            SubjectNames = string.IsNullOrWhiteSpace(x.SubjectNamesCsv)
                                ? new List<string>()
                                : x.SubjectNamesCsv
                                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(s => s.Trim())
                                    .Where(s => !string.IsNullOrWhiteSpace(s))
                                    .Distinct()
                                    .ToList()
                        }).ToList(),
                        RecentTasks = tasks
                    }
                };
            }

            if (role == "Student")
            {
                var row = await _dbContext.Database
                    .SqlQueryRaw<StudentDashboardRow>(
                        "SELECT TOP (1) * FROM dbo.vw_StudentDashboard WHERE UserId = {0}",
                        currentUser.UserId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (row == null)
                    throw new Exception("Student dashboard məlumatı tapılmadı.");

                return new DashboardResponseDto
                {
                    Role = role,
                    Student = new StudentDashboardDto
                    {
                        StudentId = row.StudentId,
                        UserId = row.UserId,
                        FullName = row.FullName ?? string.Empty,
                        StudentNumber = row.StudentNumber ?? string.Empty,
                        StudentStatus = row.StudentStatus,
                        ClassRoomId = row.ClassRoomId,
                        ClassRoomName = row.ClassRoomName,
                        MySubjectsCount = row.MySubjectsCount,
                        UpcomingExamsCount = row.UpcomingExamsCount,
                        CompletedExamsCount = row.CompletedExamsCount,
                        AverageScore = row.AverageScore,
                        UnreadNotificationsCount = row.UnreadNotificationsCount,
                        Exams = DeserializeJson<List<StudentDashboardExamItemDto>>(row.ExamsJson) ?? new List<StudentDashboardExamItemDto>(),
                        RecentTasks = DeserializeJson<List<StudentDashboardTaskItemDto>>(row.RecentTasksJson) ?? new List<StudentDashboardTaskItemDto>()
                    }
                };
            }

            throw new UnauthorizedAccessException("Bu rol üçün dashboard dəstəklənmir.");
        }

        private static T? DeserializeJson<T>(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            try
            {
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch
            {
                return default;
            }
        }

        private sealed class AdminDashboardRow
        {
            public int DashboardKey { get; set; }
            public int ActiveTeacherCount { get; set; }
            public int TodayAttendanceRate { get; set; }
            public int TotalClasses { get; set; }
            public int TotalTeachers { get; set; }
            public int TotalStudents { get; set; }
            public int ActiveExams { get; set; }
            public int TodayExamCount { get; set; }
            public int TotalSubjects { get; set; }
            public int TotalAdmins { get; set; }
            public string? ActivitiesJson { get; set; }
        }

        private sealed class TeacherDashboardRow
        {
            public int TeacherId { get; set; }
            public int UserId { get; set; }
            public string? FullName { get; set; }
            public string? Department { get; set; }
            public string? Specialization { get; set; }
            public int TeacherStatus { get; set; }
            public int TotalClasses { get; set; }
            public int TotalStudents { get; set; }
            public int TotalExams { get; set; }
            public int TotalTasks { get; set; }
            public int PendingTasks { get; set; }
            public int CompletedTasks { get; set; }
            public int TotalSubjects { get; set; }
            public int UnreadNotificationsCount { get; set; }
            public string? ClassesJson { get; set; }
            public string? RecentTasksJson { get; set; }
        }

        private sealed class TeacherDashboardClassRow
        {
            public int ClassRoomId { get; set; }
            public string? ClassRoomName { get; set; }
            public int StudentCount { get; set; }
            public int ExamCount { get; set; }
            public string? SubjectNamesCsv { get; set; }
        }

        private sealed class StudentDashboardRow
        {
            public int StudentId { get; set; }
            public int UserId { get; set; }
            public string? FullName { get; set; }
            public string? StudentNumber { get; set; }
            public int StudentStatus { get; set; }
            public int? ClassRoomId { get; set; }
            public string? ClassRoomName { get; set; }
            public int MySubjectsCount { get; set; }
            public int UpcomingExamsCount { get; set; }
            public int CompletedExamsCount { get; set; }
            public decimal AverageScore { get; set; }
            public int UnreadNotificationsCount { get; set; }
            public string? ExamsJson { get; set; }
            public string? RecentTasksJson { get; set; }
        }
    }
}
