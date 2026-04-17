using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface IAttendanceSessionRepository
    {
        Task<AttendanceSession?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<AttendanceSession?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);

        Task<List<AttendanceSession>> GetByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default);
        Task<List<AttendanceSession>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
        Task<List<AttendanceSession>> GetByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default);

        // YENI
        Task<AttendanceSession?> GetByClassRoomSubjectTeacherAndDateAsync(
            int classRoomId,
            int subjectId,
            int teacherId,
            DateTime sessionDate,
            CancellationToken cancellationToken = default);

        // YENI
        Task<List<AttendanceSession>> GetBoardSessionsAsync(
            int classRoomId,
            int subjectId,
            int teacherId,
            int year,
            int month,
            CancellationToken cancellationToken = default);

        Task AddAsync(AttendanceSession entity, CancellationToken cancellationToken = default);
        void Update(AttendanceSession entity);
        void Remove(AttendanceSession entity);
    }
}
