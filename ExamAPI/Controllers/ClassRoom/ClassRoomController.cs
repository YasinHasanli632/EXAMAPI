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

        // Yeni sinif yaradır.
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateClassRoomDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _classRoomService.CreateAsync(request, cancellationToken);

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

        // Mövcud sinifi yeniləyir.
        [HttpPut]
        public async Task<IActionResult> Update(
            [FromBody] UpdateClassRoomDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _classRoomService.UpdateAsync(request, cancellationToken);

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

        // Id-yə görə sinifi qaytarır.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            [FromRoute] int id,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _classRoomService.GetByIdAsync(id, cancellationToken);

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

        // Bütün sinifləri qaytarır.
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                var response = await _classRoomService.GetAllAsync(cancellationToken);

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

        // Id-yə görə sinif detail məlumatlarını qaytarır.
        [HttpGet("{id:int}/details")]
        public async Task<IActionResult> GetDetailsById(
            [FromRoute] int id,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _classRoomService.GetDetailsByIdAsync(id, cancellationToken);

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

        // Tələbəyə aid bütün sinifləri qaytarır.
        [HttpGet("student/{studentId:int}")]
        public async Task<IActionResult> GetByStudentId(
            [FromRoute] int studentId,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _classRoomService.GetByStudentIdAsync(studentId, cancellationToken);

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

        // Müəllimə aid bütün sinifləri qaytarır.
        [HttpGet("teacher/{teacherId:int}")]
        public async Task<IActionResult> GetByTeacherId(
            [FromRoute] int teacherId,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _classRoomService.GetByTeacherIdAsync(teacherId, cancellationToken);

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

        // Sinfə tələbə təyin edir.
        [HttpPost("assign-student")]
        public async Task<IActionResult> AssignStudent(
            [FromBody] AssignStudentToClassRoomDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _classRoomService.AssignStudentAsync(request, cancellationToken);

                return Ok(new
                {
                    message = "Tələbə sinfə uğurla təyin olundu"
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

        // Sinifdən tələbəni çıxarır.
        [HttpPost("remove-student")]
        public async Task<IActionResult> RemoveStudent(
            [FromBody] RemoveStudentFromClassRoomDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _classRoomService.RemoveStudentAsync(request, cancellationToken);

                return Ok(new
                {
                    message = "Tələbə sinifdən uğurla çıxarıldı"
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

        // Sinfə müəllim təyin edir.
        [HttpPost("assign-teacher")]
        public async Task<IActionResult> AssignTeacher(
            [FromBody] AssignTeacherToClassRoomDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _classRoomService.AssignTeacherAsync(request, cancellationToken);

                return Ok(new
                {
                    message = "Müəllim sinfə uğurla təyin olundu"
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

        // Sinifdən müəllimi çıxarır.
        [HttpPost("remove-teacher")]
        public async Task<IActionResult> RemoveTeacher(
            [FromBody] RemoveTeacherFromClassRoomDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _classRoomService.RemoveTeacherAsync(request, cancellationToken);

                return Ok(new
                {
                    message = "Müəllim sinifdən uğurla çıxarıldı"
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

        // Sinifdə olan bütün tələbələri qaytarır.
        [HttpGet("{classRoomId:int}/students")]
        public async Task<IActionResult> GetStudentsByClassRoomId(
            [FromRoute] int classRoomId,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _classRoomService.GetStudentsByClassRoomIdAsync(classRoomId, cancellationToken);

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

        // Sinifə bağlı bütün müəllimləri qaytarır.
        [HttpGet("{classRoomId:int}/teachers")]
        public async Task<IActionResult> GetTeachersByClassRoomId(
            [FromRoute] int classRoomId,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _classRoomService.GetTeachersByClassRoomIdAsync(classRoomId, cancellationToken);

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

        // Id-yə görə sinifi silir.
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            [FromRoute] int id,
            CancellationToken cancellationToken)
        {
            try
            {
                await _classRoomService.DeleteAsync(id, cancellationToken);

                return Ok(new
                {
                    message = "Sinif uğurla silindi"
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