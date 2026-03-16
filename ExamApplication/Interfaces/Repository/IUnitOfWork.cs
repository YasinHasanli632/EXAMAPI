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

        IExamRepository Exams { get; }

        IExamQuestionRepository ExamQuestions { get; }

        IExamOptionRepository ExamOptions { get; }

        IStudentExamRepository StudentExams { get; }

        IStudentAnswerRepository StudentAnswers { get; }

        IExamAccessCodeRepository ExamAccessCodes { get; }

        INotificationRepository Notifications { get; }

        IAuditLogRepository AuditLogs { get; }
        ITeacherClassRoomRepository TeacherClassRooms { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
