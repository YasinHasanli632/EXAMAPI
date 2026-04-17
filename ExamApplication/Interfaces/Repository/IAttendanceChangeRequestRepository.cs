using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    // YENI
    public interface IAttendanceChangeRequestRepository
    {
        Task<AttendanceChangeRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // YENI
        // YENI
        Task<AttendanceChangeRequest?> GetPendingByAttendanceInfoAsync(
            int classRoomId,
            int subjectId,
            int teacherId,
            int studentId,
            DateTime attendanceDate,
            CancellationToken cancellationToken = default);
        // YENI
        Task<List<AttendanceChangeRequest>> GetFilteredAsync(
            string? requestStatus,
            int? classRoomId,
            int? subjectId,
            int? teacherId,
            DateTime? attendanceDateFrom,
            DateTime? attendanceDateTo,
            CancellationToken cancellationToken = default);
        Task AddAsync(AttendanceChangeRequest entity, CancellationToken cancellationToken = default);
        void Update(AttendanceChangeRequest entity);
    }
}
