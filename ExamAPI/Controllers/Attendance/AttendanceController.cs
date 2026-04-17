using ExamApplication.DTO.Attendance;
using ExamApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamAPI.Controllers.Attendance
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        // YENI
        // Yeni session sütunu yaradır
        [HttpPost("session-column")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> CreateSessionColumn(
            [FromBody] CreateAttendanceSessionColumnDto request,
            CancellationToken cancellationToken)
        {
            var result = await _attendanceService.CreateSessionColumnAsync(request, cancellationToken);
            return Ok(result);
        }

        // YENI
        // Session üçün attendance record-ları save edir
        [HttpPut("session-records")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> SaveSessionRecords(
            [FromBody] SaveAttendanceSessionRecordsDto request,
            CancellationToken cancellationToken)
        {
            var result = await _attendanceService.SaveSessionRecordsAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _attendanceService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }

        [HttpGet("classroom/{classRoomId:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetByClassRoomId([FromRoute] int classRoomId, CancellationToken cancellationToken)
        {
            var result = await _attendanceService.GetByClassRoomIdAsync(classRoomId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("student/{studentId:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetByStudentId([FromRoute] int studentId, CancellationToken cancellationToken)
        {
            var result = await _attendanceService.GetByStudentIdAsync(studentId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("classroom/{classRoomId:int}/summary")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetStudentSummaryByClassRoomId([FromRoute] int classRoomId, CancellationToken cancellationToken)
        {
            var result = await _attendanceService.GetStudentSummaryByClassRoomIdAsync(classRoomId, cancellationToken);
            return Ok(result);
        }

        // YENI
        // Ay üzrə board qaytarır
        [HttpGet("board")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetBoard(
            [FromQuery] AttendanceBoardFilterDto filter,
            CancellationToken cancellationToken)
        {
            var result = await _attendanceService.GetBoardAsync(filter, cancellationToken);
            return Ok(result);
        }

        [HttpPost("change-request")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> CreateChangeRequest(
            [FromBody] CreateAttendanceChangeRequestDto request,
            CancellationToken cancellationToken)
        {
            var result = await _attendanceService.CreateChangeRequestAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet("change-requests")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetChangeRequests(
            [FromQuery] AttendanceChangeRequestFilterDto filter,
            CancellationToken cancellationToken)
        {
            var result = await _attendanceService.GetChangeRequestsAsync(filter, cancellationToken);
            return Ok(result);
        }

        [HttpPost("change-requests/{id:int}/approve")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> ApproveChangeRequest(
            [FromRoute] int id,
            [FromBody] ReviewAttendanceChangeRequestDto request,
            CancellationToken cancellationToken)
        {
            var result = await _attendanceService.ApproveChangeRequestAsync(id, request, cancellationToken);
            return Ok(result);
        }

        [HttpPost("change-requests/{id:int}/reject")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> RejectChangeRequest(
            [FromRoute] int id,
            [FromBody] ReviewAttendanceChangeRequestDto request,
            CancellationToken cancellationToken)
        {
            var result = await _attendanceService.RejectChangeRequestAsync(id, request, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _attendanceService.DeleteAsync(id, cancellationToken);
            return Ok(new { message = "Attendance session uğurla silindi." });
        }
    }
}