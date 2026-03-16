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

        // Yeni teacher yaradır.
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateTeacherDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _teacherService.CreateAsync(request, cancellationToken);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Mövcud teacher məlumatlarını yeniləyir.
        [HttpPut]
        public async Task<IActionResult> Update(
            [FromBody] UpdateTeacherDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _teacherService.UpdateAsync(request, cancellationToken);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Id-yə görə teacher qaytarır.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            [FromRoute] int id,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _teacherService.GetByIdAsync(id, cancellationToken);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        // UserId-yə görə teacher qaytarır.
        [HttpGet("by-user/{userId:int}")]
        public async Task<IActionResult> GetByUserId(
            [FromRoute] int userId,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _teacherService.GetByUserIdAsync(userId, cancellationToken);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        // Id-yə görə teacher detail məlumatlarını qaytarır.
        [HttpGet("{id:int}/details")]
        public async Task<IActionResult> GetDetailsById(
            [FromRoute] int id,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _teacherService.GetDetailsByIdAsync(id, cancellationToken);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        // Bütün teacher-ləri qaytarır.
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                var response = await _teacherService.GetAllAsync(cancellationToken);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Teacher-ə subject təyin edir.
        [HttpPost("assign-subject")]
        public async Task<IActionResult> AssignSubject(
            [FromBody] AssignSubjectToTeacherDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _teacherService.AssignSubjectAsync(request, cancellationToken);

                return Ok(new
                {
                    message = "Fənn müəllimə uğurla təyin olundu"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Teacher-dən subject çıxarır.
        [HttpPost("remove-subject")]
        public async Task<IActionResult> RemoveSubject(
            [FromBody] RemoveSubjectFromTeacherDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _teacherService.RemoveSubjectAsync(request, cancellationToken);

                return Ok(new
                {
                    message = "Fənn müəllimdən uğurla çıxarıldı"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Teacher-ə class room təyin edir.
        [HttpPost("assign-classroom")]
        public async Task<IActionResult> AssignClassRoom(
            [FromBody] AssignClassRoomToTeacherDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _teacherService.AssignClassRoomAsync(request, cancellationToken);

                return Ok(new
                {
                    message = "Sinif müəllimə uğurla təyin olundu"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Teacher-dən class room çıxarır.
        [HttpPost("remove-classroom")]
        public async Task<IActionResult> RemoveClassRoom(
            [FromBody] RemoveClassRoomFromTeacherDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _teacherService.RemoveClassRoomAsync(request, cancellationToken);

                return Ok(new
                {
                    message = "Sinif müəllimdən uğurla çıxarıldı"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Teacher-in bütün subject-lərini qaytarır.
        [HttpGet("{teacherId:int}/subjects")]
        public async Task<IActionResult> GetSubjectsByTeacherId(
            [FromRoute] int teacherId,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _teacherService.GetSubjectsByTeacherIdAsync(teacherId, cancellationToken);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        // Teacher-in bütün class room-larını qaytarır.
        [HttpGet("{teacherId:int}/classrooms")]
        public async Task<IActionResult> GetClassRoomsByTeacherId(
            [FromRoute] int teacherId,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _teacherService.GetClassRoomsByTeacherIdAsync(teacherId, cancellationToken);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        // Teacher-i silir.
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            [FromRoute] int id,
            CancellationToken cancellationToken)
        {
            try
            {
                await _teacherService.DeleteAsync(id, cancellationToken);

                return Ok(new
                {
                    message = "Müəllim uğurla silindi"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}