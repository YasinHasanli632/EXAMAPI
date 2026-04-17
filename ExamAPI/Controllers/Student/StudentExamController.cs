using ExamApplication.DTO.Student;
using ExamApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamAPI.Controllers.Student
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class StudentExamController : ControllerBase
    {
        private readonly IStudentExamService _studentExamService;

        public StudentExamController(IStudentExamService studentExamService)
        {
            _studentExamService = studentExamService;
        }

        // YENI
        // Cari student üçün bütün exam siyahısı
        [HttpGet("my")]
        public async Task<IActionResult> GetMyExams(CancellationToken cancellationToken)
        {
            var result = await _studentExamService.GetMyExamsAsync(cancellationToken);
            return Ok(result);
        }

        // YENI
        // Müəyyən exam detail/rules page
        [HttpGet("{examId:int}")]
        public async Task<IActionResult> GetMyExamDetail([FromRoute] int examId, CancellationToken cancellationToken)
        {
            var result = await _studentExamService.GetMyExamDetailAsync(examId, cancellationToken);
            return Ok(result);
        }

        // YENI
        // Access code verify
        [HttpPost("verify-access-code")]
        public async Task<IActionResult> VerifyAccessCode(
            [FromBody] VerifyStudentExamAccessCodeRequestDto request,
            CancellationToken cancellationToken)
        {
            var result = await _studentExamService.VerifyAccessCodeAsync(request, cancellationToken);

            return Ok(new
            {
                success = result,
                message = "Giriş kodu uğurla təsdiqləndi."
            });
        }

        // YENI
        // Exam başlat
        [HttpPost("start")]
        public async Task<IActionResult> StartExam(
            [FromBody] StartStudentExamRequestDto request,
            CancellationToken cancellationToken)
        {
            var result = await _studentExamService.StartExamAsync(request, cancellationToken);
            return Ok(result);
        }

        // YENI
        // Cavabı save et
        [HttpPost("save-answer")]
        public async Task<IActionResult> SaveAnswer(
            [FromBody] SaveStudentAnswerRequestDto request,
            CancellationToken cancellationToken)
        {
            var result = await _studentExamService.SaveAnswerAsync(request, cancellationToken);
            return Ok(result);
        }

        // YENI
        // Exam submit et
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitExam(
            [FromBody] SubmitStudentExamRequestDto request,
            CancellationToken cancellationToken)
        {
            var result = await _studentExamService.SubmitExamAsync(request, cancellationToken);
            return Ok(result);
        }

        // YENI
        // Student yalnız open question olmayan exam review-nu görə bilsin
        [HttpGet("my/{studentExamId:int}/review")]
        public async Task<IActionResult> GetMyExamReview(
            [FromRoute] int studentExamId,
            CancellationToken cancellationToken)
        {
            var result = await _studentExamService.GetMyExamReviewAsync(studentExamId, cancellationToken);
            return Ok(result);
        }

        // YENI
        // Security event log
        [HttpPost("security-log")]
        public async Task<IActionResult> LogSecurityEvent(
            [FromBody] LogExamSecurityEventRequestDto request,
            CancellationToken cancellationToken)
        {
            await _studentExamService.LogSecurityEventAsync(request, cancellationToken);

            return Ok(new
            {
                message = "Security event uğurla qeydə alındı."
            });
        }
    }
}