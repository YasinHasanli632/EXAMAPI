using ExamApplication.DTO.Attendance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    public interface IAttendanceService
    {
        // YENI
        Task<AttendanceSessionDetailDto> CreateSessionColumnAsync(
            CreateAttendanceSessionColumnDto request,
            CancellationToken cancellationToken = default);

        // YENI
        Task<AttendanceSessionDetailDto> SaveSessionRecordsAsync(
            SaveAttendanceSessionRecordsDto request,
            CancellationToken cancellationToken = default);

        Task<AttendanceSessionDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<List<AttendanceSessionDto>> GetByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default);

        Task<List<AttendanceStudentHistoryDto>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);

        Task<List<StudentAttendanceSummaryDto>> GetStudentSummaryByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default);

        // YENI
        Task<AttendanceBoardDto> GetBoardAsync(
            AttendanceBoardFilterDto filter,
            CancellationToken cancellationToken = default);

        Task<AttendanceChangeRequestDto> CreateChangeRequestAsync(
            CreateAttendanceChangeRequestDto request,
            CancellationToken cancellationToken = default);

        Task<List<AttendanceChangeRequestDto>> GetChangeRequestsAsync(
            AttendanceChangeRequestFilterDto filter,
            CancellationToken cancellationToken = default);

        Task<AttendanceChangeRequestDto> ApproveChangeRequestAsync(
            int changeRequestId,
            ReviewAttendanceChangeRequestDto request,
            CancellationToken cancellationToken = default);

        Task<AttendanceChangeRequestDto> RejectChangeRequestAsync(
            int changeRequestId,
            ReviewAttendanceChangeRequestDto request,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
