using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface IExamRepository
    {
        Task<Exam?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<Exam?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);

        Task<List<Exam>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<List<Exam>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);

        Task<List<Exam>> GetByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default);

        Task<List<Exam>> GetBySubjectIdAsync(int subjectId, CancellationToken cancellationToken = default);

        Task<List<Exam>> GetByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default);

        Task<List<Exam>> GetActiveExamsAsync(CancellationToken cancellationToken = default);

        Task<List<Exam>> GetPublishedByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default);

        Task AddAsync(Exam exam, CancellationToken cancellationToken = default);

        void Update(Exam exam);

        void Remove(Exam exam);

        // YENI
        Task<List<Exam>> GetPublishedUpcomingExamsAsync(DateTime now, DateTime until, CancellationToken cancellationToken = default);

        // YENI
        Task<List<Exam>> GetPublishedActiveExamsAsync(DateTime now, CancellationToken cancellationToken = default);

        // YENI
        Task<List<Exam>> GetPublishedEndedExamsAsync(DateTime now, CancellationToken cancellationToken = default);

        // YENI
        Task<List<Exam>> GetByTeacherIdsAsync(List<int> teacherIds, CancellationToken cancellationToken = default);

        // YENI
        Task<List<Exam>> GetByTeacherIdWithFullStatsAsync(int teacherId, CancellationToken cancellationToken = default);
    }
}
