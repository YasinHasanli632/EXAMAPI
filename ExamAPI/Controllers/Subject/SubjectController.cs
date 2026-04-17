using ExamApplication.DTO.Subject;
using ExamApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExamAPI.Controllers.Subject
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        /// <summary>
        /// Yeni subject yaradır.
        /// Admin və SuperAdmin istifadə edə bilər.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> Create([FromBody] CreateSubjectDto request, CancellationToken cancellationToken)
        {
            var result = await _subjectService.CreateAsync(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Mövcud subject-i yeniləyir.
        /// Admin və SuperAdmin istifadə edə bilər.
        /// </summary>
        [HttpPut]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> Update([FromBody] UpdateSubjectDto request, CancellationToken cancellationToken)
        {
            var result = await _subjectService.UpdateAsync(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Bütün subject-ləri sadə siyahı kimi qaytarır.
        /// Teacher create/edit dropdown üçün əsas endpoint budur.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _subjectService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Bütün subject-ləri detail ilə birlikdə qaytarır.
        /// Frontend subject list/detail səhifələri üçün uyğundur.
        /// </summary>
        [HttpGet("details")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetAllDetails(CancellationToken cancellationToken)
        {
            var result = await _subjectService.GetAllDetailsAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Id-yə görə subject gətirir.
        /// </summary>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _subjectService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Id-yə görə subject detail gətirir.
        /// Teacher əlaqələri də içində olur.
        /// </summary>
        [HttpGet("{id:int}/details")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetDetailsById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _subjectService.GetDetailsByIdAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Subject-ə bağlı bütün teacher-ləri gətirir.
        /// Frontend detail səhifəsində ayrıca istifadə oluna bilər.
        /// </summary>
        [HttpGet("{subjectId:int}/teachers")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetTeachersBySubjectId([FromRoute] int subjectId, CancellationToken cancellationToken)
        {
            var result = await _subjectService.GetTeachersBySubjectIdAsync(subjectId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Subject statusunu dəyişir.
        /// PATCH: /api/Subject/change-status
        /// </summary>
        [HttpPatch("change-status")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> ChangeStatus([FromBody] ChangeSubjectStatusDto request, CancellationToken cancellationToken)
        {
            await _subjectService.ChangeStatusAsync(request, cancellationToken);
            return Ok(new { message = "Subject status uğurla yeniləndi." });
        }

        /// <summary>
        /// Subject-ə teacher-ləri toplu şəkildə sync edir.
        /// Mövcud əlaqələr düzəldilir, artıq olanlar silinir, yenilər əlavə olunur.
        /// </summary>
        [HttpPut("sync-teachers")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> SyncTeachers([FromBody] SyncSubjectTeachersDto request, CancellationToken cancellationToken)
        {
            await _subjectService.SyncTeachersAsync(request, cancellationToken);
            return Ok(new { message = "Subject teacher əlaqələri uğurla sinxronlaşdırıldı." });
        }

        /// <summary>
        /// Subject-i silir.
        /// Əgər teacher əlaqəsi varsa service exception atacaq.
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "IsSuperAdmin")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _subjectService.DeleteAsync(id, cancellationToken);
            return Ok(new { message = "Subject uğurla silindi." });
        }
    }
}
