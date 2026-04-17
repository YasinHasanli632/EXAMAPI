using ExamApplication.DTO.Student;
using ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamAPI.Controllers.Student
{
    // YENI - SAGIRD UCUN
    [Route("api/student/attendance")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class StudentAttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public StudentAttendanceController(
            IAttendanceService attendanceService,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _attendanceService = attendanceService;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        // YENI - SAGIRD UCUN
        [HttpGet]
        [ProducesResponseType(typeof(List<StudentAttendanceItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyAttendance(CancellationToken cancellationToken)
        {
            var currentUser = _currentUserService.GetCurrentUser();
            var userId = currentUser?.UserId;

            if (userId == null)
            {
                return Unauthorized(new { message = "Cari istifadəçi tapılmadı." });
            }

            var student = await _unitOfWork.Students.GetByUserIdAsync(userId.Value, cancellationToken);

            if (student == null)
            {
                return NotFound(new { message = "Şagird profili tapılmadı." });
            }

            var attendanceHistory = await _attendanceService.GetByStudentIdAsync(student.Id, cancellationToken);

            var result = attendanceHistory.Select(x => new StudentAttendanceItemDto
            {
                AttendanceSessionId = x.AttendanceSessionId,
                SessionDate = x.SessionDate,
                ClassName = x.ClassName,
                SubjectName = x.SubjectName,
                TeacherName = x.TeacherName,
                Status = x.Status,
                Notes = x.Notes,
                AbsenceReasonType = x.AbsenceReasonType,
                AbsenceReasonNote = x.AbsenceReasonNote,
                LateArrivalTime = x.LateArrivalTime,
                LateNote = x.LateNote
            }).ToList();

            return Ok(result);
        }
    }
}