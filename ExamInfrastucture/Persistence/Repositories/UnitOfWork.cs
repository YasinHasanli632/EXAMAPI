using ExamApplication.Interfaces.Repository;
using ExamInfrastucture.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamInfrastucture.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IUserRepository Users { get; }
        public IStudentRepository Students { get; }
        public ITeacherRepository Teachers { get; }
        public IClassRoomRepository ClassRooms { get; }
        public ISubjectRepository Subjects { get; }
        public ITeacherSubjectRepository TeacherSubjects { get; }
        public IStudentClassRepository StudentClasses { get; }
        public IExamRepository Exams { get; }
        public IExamQuestionRepository ExamQuestions { get; }
        public IExamOptionRepository ExamOptions { get; }
        public IStudentExamRepository StudentExams { get; }
        public IStudentAnswerRepository StudentAnswers { get; }
        public IExamAccessCodeRepository ExamAccessCodes { get; }
        public INotificationRepository Notifications { get; }
        public IAuditLogRepository AuditLogs { get; }
        public ITeacherClassRoomRepository TeacherClassRooms { get; }

        public UnitOfWork(
            AppDbContext context,
            IUserRepository users,
            IStudentRepository students,
            ITeacherRepository teachers,
            IClassRoomRepository classRooms,
            ISubjectRepository subjects,
            ITeacherSubjectRepository teacherSubjects,
            IStudentClassRepository studentClasses,
            IExamRepository exams,
            IExamQuestionRepository examQuestions,
            IExamOptionRepository examOptions,
            IStudentExamRepository studentExams,
            IStudentAnswerRepository studentAnswers,
            IExamAccessCodeRepository examAccessCodes,
            INotificationRepository notifications,
            IAuditLogRepository auditLogs, ITeacherClassRoomRepository teacherClassRooms)
        {
            _context = context;
            Users = users;
            Students = students;
            Teachers = teachers;
            ClassRooms = classRooms;
            Subjects = subjects;
            TeacherSubjects = teacherSubjects;
            StudentClasses = studentClasses;
            Exams = exams;
            ExamQuestions = examQuestions;
            ExamOptions = examOptions;
            StudentExams = studentExams;
            StudentAnswers = studentAnswers;
            ExamAccessCodes = examAccessCodes;
            Notifications = notifications;
            AuditLogs = auditLogs;
            TeacherClassRooms = teacherClassRooms;
        }

        // Bütün pending dəyişiklikləri database-ə yazır
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        // Managed resource-ları sərbəst buraxır
        public void Dispose()
        {
            _context.Dispose();
        }

        // Async şəkildə managed resource-ları sərbəst buraxır
        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}
