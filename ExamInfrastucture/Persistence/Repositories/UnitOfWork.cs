using ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Repository.ExamApplication.Interfaces.Repository;
using ExamDomain.Entities;
using ExamInfrastucture.DAL;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExamInfrastucture.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private bool _disposed;

        public UnitOfWork(
            AppDbContext context,
            IUserRepository users,
            IStudentRepository students,
            ITeacherRepository teachers,
            IClassRoomRepository classRooms,
            ISubjectRepository subjects,
            ITeacherSubjectRepository teacherSubjects,
            IStudentClassRepository studentClasses,
            IClassTeacherSubjectRepository classTeacherSubjects,
            IExamRepository exams,
            IExamQuestionRepository examQuestions,
            IExamOptionRepository examOptions,
            IStudentExamRepository studentExams,
            IStudentAnswerRepository studentAnswers,
            IStudentAnswerOptionRepository studentAnswerOptions,
            IExamAccessCodeRepository examAccessCodes,
            INotificationRepository notifications,
            IAuditLogRepository auditLogs,
            ITeacherTaskRepository teacherTasks,
            IStudentTaskRepository studentTasks,
            IExamSecurityLogRepository examSecurityLogs,
            IAttendanceSessionRepository attendanceSessions,
            IAttendanceRecordRepository attendanceRecords,
            ISystemSettingRepository systemSettings,
            IAttendanceChangeRequestRepository attendanceChangeRequest,
            IRefreshTokenRepository refreshTokens) // YENI
        {
            _context = context;

            Users = users;
            Students = students;
            Teachers = teachers;
            ClassRooms = classRooms;
            Subjects = subjects;
            TeacherSubjects = teacherSubjects;
            StudentClasses = studentClasses;
            ClassTeacherSubjects = classTeacherSubjects;
            Exams = exams;
            ExamQuestions = examQuestions;
            ExamOptions = examOptions;
            StudentExams = studentExams;
            StudentAnswers = studentAnswers;
            StudentAnswerOptions = studentAnswerOptions;
            ExamAccessCodes = examAccessCodes;
            Notifications = notifications;
            AuditLogs = auditLogs;
            TeacherTasks = teacherTasks;
            StudentTasks = studentTasks;
            AttendanceSessions = attendanceSessions;
            AttendanceRecords = attendanceRecords;
            AttendanceChangeRequest = attendanceChangeRequest;
            ExamSecurityLogs = examSecurityLogs;
            SystemSettings = systemSettings;
            RefreshTokens = refreshTokens; // YENI
        }

        public IUserRepository Users { get; }
        public IStudentRepository Students { get; }
        public ITeacherRepository Teachers { get; }
        public IClassRoomRepository ClassRooms { get; }
        public ISubjectRepository Subjects { get; }
        public ITeacherSubjectRepository TeacherSubjects { get; }
        public IStudentClassRepository StudentClasses { get; }
        public IClassTeacherSubjectRepository ClassTeacherSubjects { get; }
        public IExamRepository Exams { get; }
        public IExamQuestionRepository ExamQuestions { get; }
        public IExamOptionRepository ExamOptions { get; }
        public IStudentExamRepository StudentExams { get; }
        public IStudentAnswerRepository StudentAnswers { get; }

        // YENI
        public IStudentAnswerOptionRepository StudentAnswerOptions { get; }

        // YENI
        public ISystemSettingRepository SystemSettings { get; }

        public IStudentTaskRepository StudentTasks { get; }

        public IExamAccessCodeRepository ExamAccessCodes { get; }
        public INotificationRepository Notifications { get; }
        public IAuditLogRepository AuditLogs { get; }
        public ITeacherTaskRepository TeacherTasks { get; }

        // YENI
        public IAttendanceSessionRepository AttendanceSessions { get; }

        // YENI
        public IAttendanceRecordRepository AttendanceRecords { get; }

        // YENI
        public IAttendanceChangeRequestRepository AttendanceChangeRequest { get; }

        // YENI
        public IExamSecurityLogRepository ExamSecurityLogs { get; }

        // YENI
        public IRefreshTokenRepository RefreshTokens { get; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            await _context.DisposeAsync();
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _context.Dispose();
            }

            _disposed = true;
        }
    }
}