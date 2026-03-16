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
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // Login olmuş student-in profil məlumatlarını qaytarır.
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetCurrentUserId();

                var response = await _studentService.GetStudentProfileAsync(userId, cancellationToken);

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

        // Login olmuş student-in qoşulduğu sinifləri qaytarır.
        [HttpGet("classes")]
        public async Task<IActionResult> GetClasses(CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetCurrentUserId();

                var response = await _studentService.GetStudentClassesAsync(userId, cancellationToken);

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

        // Login olmuş student üçün əlçatan imtahanları qaytarır.
        [HttpGet("exams")]
        public async Task<IActionResult> GetAvailableExams(CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetCurrentUserId();

                var response = await _studentService.GetAvailableExamsAsync(userId, cancellationToken);

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

        // Login olmuş student üçün konkret imtahanın detail məlumatını qaytarır.
        [HttpGet("exams/{examId:int}")]
        public async Task<IActionResult> GetExamDetail(
            [FromRoute] int examId,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetCurrentUserId();

                var response = await _studentService.GetExamDetailAsync(userId, examId, cancellationToken);

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

        // Login olmuş student üçün imtahan sessiyası başladır.
        [HttpPost("exams/start")]
        public async Task<IActionResult> StartExam(
            [FromBody] StartStudentExamRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                request.UserId = GetCurrentUserId();

                var response = await _studentService.StartExamAsync(request, cancellationToken);

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

        // Student-in suala verdiyi cavabı save edir və ya update edir.
        [HttpPost("answers/save")]
        public async Task<IActionResult> SaveAnswer(
            [FromBody] SaveStudentAnswerRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _studentService.SaveAnswerAsync(request, cancellationToken);

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

        // Verilən student exam session-a aid bütün cavabları qaytarır.
        [HttpGet("sessions/{studentExamId:int}/answers")]
        public async Task<IActionResult> GetExamAnswers(
            [FromRoute] int studentExamId,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _studentService.GetExamAnswersAsync(studentExamId, cancellationToken);

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

        // Student imtahanı submit edir.
        [HttpPost("exams/submit")]
        public async Task<IActionResult> SubmitExam(
            [FromBody] SubmitStudentExamRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _studentService.SubmitExamAsync(request, cancellationToken);

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

        // Login olmuş student-in imtahan tarixçəsini qaytarır.
        [HttpGet("history")]
        public async Task<IActionResult> GetExamHistory(CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetCurrentUserId();

                var response = await _studentService.GetExamHistoryAsync(userId, cancellationToken);

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

        // Login olmuş student üçün konkret imtahan nəticəsini qaytarır.
        [HttpGet("results/{studentExamId:int}")]
        public async Task<IActionResult> GetExamResult(
            [FromRoute] int studentExamId,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetCurrentUserId();

                var response = await _studentService.GetExamResultAsync(userId, studentExamId, cancellationToken);

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

        // Token içindən cari login olmuş istifadəçinin Id dəyərini çıxarır.
        private int GetCurrentUserId()
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("nameid") ??
                User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(userIdValue))
                throw new Exception("Token içində istifadəçi identifikatoru tapılmadı");

            if (!int.TryParse(userIdValue, out var userId))
                throw new Exception("Token içində istifadəçi identifikatoru düzgün formatda deyil");

            return userId;
        }
    }
}
