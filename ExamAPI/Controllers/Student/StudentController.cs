using ExamApplication.DTO.Student;
using ExamApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExamAPI.Controllers.Student
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentController : ControllerBase
    {
        private readonly IStudentAdminService _studentAdminService;

        public StudentController(IStudentAdminService studentAdminService)
        {
            _studentAdminService = studentAdminService;
        }

        // Admin panel üçün bütün student-ləri gətirir
        [HttpGet]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _studentAdminService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        // Id-yə görə student detail gətirir
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _studentAdminService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }

        // Yeni student yaradır
        [HttpPost]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> Create([FromBody] CreateStudentDto request, CancellationToken cancellationToken)
        {
            var result = await _studentAdminService.CreateAsync(request, cancellationToken);
            return Ok(result);
        }

        // Mövcud student-i yeniləyir
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStudentDto request, CancellationToken cancellationToken)
        {
            request.Id = id;
            var result = await _studentAdminService.UpdateAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _studentAdminService.DeleteAsync(id, cancellationToken);
            return Ok(new { message = "Şagird uğurla deaktiv edildi." });
        }

        // Dropdown/select üçün student option-larını gətirir
        [HttpGet("options")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetOptions(CancellationToken cancellationToken)
        {
            var result = await _studentAdminService.GetOptionsAsync(cancellationToken);
            return Ok(result);
        }

        // Axtarışlı student option endpoint-i
        [HttpGet("search")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> Search([FromQuery] string? search, CancellationToken cancellationToken)
        {
            var result = await _studentAdminService.SearchAsync(search, cancellationToken);
            return Ok(result);
        }
        [HttpGet("{id:int}/tasks")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetTasks([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _studentAdminService.GetTasksAsync(id, cancellationToken);
            return Ok(result);
        }

        [HttpPost("{id:int}/tasks")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> CreateTask([FromRoute] int id, [FromBody] CreateStudentTaskDto request, CancellationToken cancellationToken)
        {
            var result = await _studentAdminService.CreateTaskAsync(id, request, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:int}/tasks/{taskId:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> UpdateTask([FromRoute] int id, [FromRoute] int taskId, [FromBody] UpdateStudentTaskDto request, CancellationToken cancellationToken)
        {
            request.Id = taskId;
            var result = await _studentAdminService.UpdateTaskAsync(id, request, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:int}/tasks/{taskId:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> DeleteTask([FromRoute] int id, [FromRoute] int taskId, CancellationToken cancellationToken)
        {
            await _studentAdminService.DeleteTaskAsync(id, taskId, cancellationToken);
            return Ok(new { message = "Task uğurla silindi." });
        }

        [HttpGet("{id:int}/exam-reviews/{studentExamId:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetExamReview([FromRoute] int id, [FromRoute] int studentExamId, CancellationToken cancellationToken)
        {
            var result = await _studentAdminService.GetExamReviewAsync(id, studentExamId, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:int}/attendance/{attendanceSessionId:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> UpdateAttendance(
    [FromRoute] int id,
    [FromRoute] int attendanceSessionId,
    [FromBody] UpdateStudentAttendanceRecordDto request,
    CancellationToken cancellationToken)
        {
            request.AttendanceSessionId = attendanceSessionId;

            var result = await _studentAdminService.UpdateAttendanceAsync(id, request, cancellationToken);
            return Ok(result);
        }
    }
}
