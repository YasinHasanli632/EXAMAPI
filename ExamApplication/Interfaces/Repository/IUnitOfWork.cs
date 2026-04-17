using ExamApplication.Interfaces.Repository.ExamApplication.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IUserRepository Users { get; }
        IStudentRepository Students { get; }
        ITeacherRepository Teachers { get; }
        IClassRoomRepository ClassRooms { get; }
        ISubjectRepository Subjects { get; }
        ITeacherSubjectRepository TeacherSubjects { get; }
        IStudentClassRepository StudentClasses { get; }
        IClassTeacherSubjectRepository ClassTeacherSubjects { get; }
        IExamRepository Exams { get; }
        IExamQuestionRepository ExamQuestions { get; }
        IExamOptionRepository ExamOptions { get; }
        IStudentExamRepository StudentExams { get; }
        IStudentAnswerRepository StudentAnswers { get; }

        // YENI
        IStudentAnswerOptionRepository StudentAnswerOptions { get; }

        // YENI
        ISystemSettingRepository SystemSettings { get; }

        IExamAccessCodeRepository ExamAccessCodes { get; }
        INotificationRepository Notifications { get; }
        IAuditLogRepository AuditLogs { get; }
        ITeacherTaskRepository TeacherTasks { get; }

        // YENI
        IAttendanceSessionRepository AttendanceSessions { get; }

        // YENI
        IExamSecurityLogRepository ExamSecurityLogs { get; }

        // YENI
        IAttendanceRecordRepository AttendanceRecords { get; }

        IStudentTaskRepository StudentTasks { get; } // YENI
        IAttendanceChangeRequestRepository AttendanceChangeRequest { get; }

        // YENI
        IRefreshTokenRepository RefreshTokens { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
