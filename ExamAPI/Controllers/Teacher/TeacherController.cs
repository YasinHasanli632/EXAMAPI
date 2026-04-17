using ExamApplication.DTO.Teacher;
using ExamApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExamAPI.Controllers.Teacher
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        // Yeni teacher yaradır
        [HttpPost]
        [Authorize(Roles = "IsSuperAdmin")]
        public async Task<IActionResult> Create([FromBody] CreateTeacherDto request, CancellationToken cancellationToken)
        {
            var result = await _teacherService.CreateAsync(request, cancellationToken);
            return Ok(result);
        }

        // Mövcud teacher-i yeniləyir
        [HttpPut]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> Update([FromBody] UpdateTeacherDto request, CancellationToken cancellationToken)
        {
            var result = await _teacherService.UpdateAsync(request, cancellationToken);
            return Ok(result);
        }

        // Id-yə görə teacher gətirir
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _teacherService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }

        // UserId-yə görə teacher gətirir
        [HttpGet("user/{userId:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetByUserId(int userId, CancellationToken cancellationToken)
        {
            var result = await _teacherService.GetByUserIdAsync(userId, cancellationToken);
            return Ok(result);
        }

        // Id-yə görə teacher detail gətirir
        [HttpGet("{id:int}/details")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetDetailsById(int id, CancellationToken cancellationToken)
        {
            var result = await _teacherService.GetDetailsByIdAsync(id, cancellationToken);
            return Ok(result);
        }

        // YENI
        // UserId-yə görə teacher detail gətirir
        [HttpGet("user/{userId:int}/details")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetDetailsByUserId(int userId, CancellationToken cancellationToken)
        {
            var result = await _teacherService.GetDetailsByUserIdAsync(userId, cancellationToken);
            return Ok(result);
        }

        // Bütün teacher-ləri gətirir
        [HttpGet]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _teacherService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        // YENI
        // Bütün teacher-ləri detail ilə gətirir
        [HttpGet("details")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> GetAllDetails(CancellationToken cancellationToken)
        {
            var result = await _teacherService.GetAllDetailsAsync(cancellationToken);
            return Ok(result);
        }

        // Teacher-ə subject bağlayır
        [HttpPost("assign-subject")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> AssignSubject([FromBody] AssignSubjectToTeacherDto request, CancellationToken cancellationToken)
        {
            await _teacherService.AssignSubjectAsync(request, cancellationToken);
            return Ok(new { message = "Subject teacher-ə uğurla bağlandı." });
        }

        // Teacher-dən subject çıxarır
        [HttpDelete("remove-subject")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> RemoveSubject([FromBody] RemoveSubjectFromTeacherDto request, CancellationToken cancellationToken)
        {
            await _teacherService.RemoveSubjectAsync(request, cancellationToken);
            return Ok(new { message = "Subject teacher-dən uğurla çıxarıldı." });
        }

        // Teacher-ə class bağlayır
        [HttpPost("assign-classroom")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> AssignClassRoom([FromBody] AssignClassRoomToTeacherDto request, CancellationToken cancellationToken)
        {
            await _teacherService.AssignClassRoomAsync(request, cancellationToken);
            return Ok(new { message = "ClassRoom teacher-ə uğurla bağlandı." });
        }

        // Teacher-dən class çıxarır
        [HttpDelete("remove-classroom")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> RemoveClassRoom([FromBody] RemoveClassRoomFromTeacherDto request, CancellationToken cancellationToken)
        {
            await _teacherService.RemoveClassRoomAsync(request, cancellationToken);
            return Ok(new { message = "ClassRoom teacher-dən uğurla çıxarıldı." });
        }

        // Teacher-in bütün subject-lərini gətirir
        [HttpGet("{teacherId:int}/subjects")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetSubjectsByTeacherId(int teacherId, CancellationToken cancellationToken)
        {
            var result = await _teacherService.GetSubjectsByTeacherIdAsync(teacherId, cancellationToken);
            return Ok(result);
        }

        // Teacher-in bütün class-larını gətirir
        [HttpGet("{teacherId:int}/classrooms")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetClassRoomsByTeacherId(int teacherId, CancellationToken cancellationToken)
        {
            var result = await _teacherService.GetClassRoomsByTeacherIdAsync(teacherId, cancellationToken);
            return Ok(result);
        }

        // Teacher-i silir
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _teacherService.DeleteAsync(id, cancellationToken);
            return Ok(new { message = "Teacher uğurla silindi." });
        }

        // YENI
        // Teacher status dəyişir
        [HttpPatch("change-status")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> ChangeStatus([FromBody] ChangeTeacherStatusDto request, CancellationToken cancellationToken)
        {
            await _teacherService.ChangeStatusAsync(request, cancellationToken);
            return Ok(new { message = "Teacher status uğurla yeniləndi." });
        }

        // YENI
        // Teacher subject-lərini full sync edir
        [HttpPut("sync-subjects")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> SyncSubjects([FromBody] SyncTeacherSubjectsDto request, CancellationToken cancellationToken)
        {
            await _teacherService.SyncSubjectsAsync(request, cancellationToken);
            return Ok(new { message = "Teacher subject-ləri uğurla sinxronlaşdırıldı." });
        }

        // YENI
        // Teacher class-room assignment-lərini full sync edir
        [HttpPut("sync-classrooms")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> SyncClassRooms([FromBody] SyncTeacherClassRoomsDto request, CancellationToken cancellationToken)
        {
            await _teacherService.SyncClassRoomsAsync(request, cancellationToken);
            return Ok(new { message = "Teacher class-room assignment-ləri uğurla sinxronlaşdırıldı." });
        }

        // YENI
        // Teacher task-larını gətirir
        [HttpGet("{teacherId:int}/tasks")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetTasksByTeacherId(int teacherId, CancellationToken cancellationToken)
        {
            var result = await _teacherService.GetTasksByTeacherIdAsync(teacherId, cancellationToken);
            return Ok(result);
        }

        // YENI
        // Teacher overview stats gətirir
        [HttpGet("{teacherId:int}/overview-stats")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetOverviewStats(int teacherId, CancellationToken cancellationToken)
        {
            var result = await _teacherService.GetOverviewStatsAsync(teacherId, cancellationToken);
            return Ok(result);
        }
        // YENI
        [HttpGet("me")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
        {
            var result = await _teacherService.GetMeAsync(cancellationToken);
            return Ok(result);
        }

        // YENI
        [HttpGet("me/dashboard")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetMyDashboard(CancellationToken cancellationToken)
        {
            var result = await _teacherService.GetMyDashboardAsync(cancellationToken);
            return Ok(result);
        }

        // YENI
        [HttpGet("me/classrooms")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetMyClassRooms(CancellationToken cancellationToken)
        {
            var result = await _teacherService.GetMyClassRoomsAsync(cancellationToken);
            return Ok(result);
        }

        // YENI
        [HttpGet("me/classrooms/{classRoomId:int}/details")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetMyClassRoomDetails(int classRoomId, CancellationToken cancellationToken)
        {
            var result = await _teacherService.GetMyClassRoomDetailsAsync(classRoomId, cancellationToken);
            return Ok(result);
        }
    }
}
