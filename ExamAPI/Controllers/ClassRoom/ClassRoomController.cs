using ExamApplication.DTO.Class;
using ExamApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExamAPI.Controllers.ClassRoom
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClassRoomController : ControllerBase
    {
        private readonly IClassRoomService _classRoomService;

        public ClassRoomController(IClassRoomService classRoomService)
        {
            _classRoomService = classRoomService;
        }

        // Bütün sinifləri gətirir
        [HttpGet]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _classRoomService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        // Id-yə görə sinif detail gətirir
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _classRoomService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }

        // Yeni sinif yaradır
        [HttpPost]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> Create([FromBody] CreateClassDto request, CancellationToken cancellationToken)
        {
            var result = await _classRoomService.CreateAsync(request, cancellationToken);
            return Ok(result);
        }

        // Mövcud sinifi yeniləyir
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateClassDto request, CancellationToken cancellationToken)
        {
            request.Id = id;
            var result = await _classRoomService.UpdateAsync(request, cancellationToken);
            return Ok(result);
        }

        // Sinifi silir
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _classRoomService.DeleteAsync(id, cancellationToken);
            return Ok(new { message = "Sinif uğurla silindi." });
        }

        // Class create/edit üçün student axtarışı
        [HttpGet("student-options")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> SearchStudents([FromQuery] string? search, CancellationToken cancellationToken)
        {
            var result = await _classRoomService.SearchStudentsAsync(search, cancellationToken);
            return Ok(result);
        }

        // Class create/edit üçün teacher axtarışı
        [HttpGet("teacher-options")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> SearchTeachers([FromQuery] string? search, CancellationToken cancellationToken)
        {
            var result = await _classRoomService.SearchTeachersAsync(search, cancellationToken);
            return Ok(result);
        }

        // Class create/edit üçün subject dropdown-u
        [HttpGet("subject-options")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetSubjectOptions(CancellationToken cancellationToken)
        {
            var result = await _classRoomService.GetSubjectOptionsAsync(cancellationToken);
            return Ok(result);
        }
    }
}