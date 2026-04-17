using ExamApplication.DTO.Exam;
using ExamApplication.DTO.Teacher; // YENI
using ExamApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamAPI.Controllers.Exam
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        // Bütün imtahanları gətirir
        [HttpGet]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _examService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        // Id-yə görə imtahan detail gətirir
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _examService.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }

        // Yeni imtahan yaradır
        [HttpPost]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> Create([FromBody] CreateExamDto request, CancellationToken cancellationToken)
        {
            var result = await _examService.CreateAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> Update(
     [FromRoute] int id,
     [FromBody] UpdateExamDto request,
     CancellationToken cancellationToken)
        {
            try
            {
                request.Id = id;

                var result = await _examService.UpdateAsync(request, cancellationToken);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // İmtahanı publish edir
        [HttpPatch("{id:int}/publish")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> Publish([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _examService.PublishAsync(id, cancellationToken);
            return Ok(new { message = "İmtahan uğurla publish edildi." });
        }

        // İmtahanı silir
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            try
            {
                await _examService.DeleteAsync(id, cancellationToken);
                return Ok(new { message = "İmtahan uğurla silindi." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Exam create/edit üçün class dropdown-u
        [HttpGet("class-options")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetClassOptions(CancellationToken cancellationToken)
        {
            var result = await _examService.GetClassOptionsAsync(cancellationToken);
            return Ok(result);
        }

        // Exam create/edit üçün subject dropdown-u
        [HttpGet("subject-options")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetSubjectOptions(CancellationToken cancellationToken)
        {
            var result = await _examService.GetSubjectOptionsAsync(cancellationToken);
            return Ok(result);
        }

        // Exam create/edit üçün teacher dropdown-u
        [HttpGet("teacher-options")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetTeacherOptions(CancellationToken cancellationToken)
        {
            var result = await _examService.GetTeacherOptionsAsync(cancellationToken);
            return Ok(result);
        }

        // YENI
        // Teacher özü üçün exam create səhifəsində lazım olan seçimləri gətirir
        [HttpGet("teacher/my-create-options")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetTeacherCreateOptions(CancellationToken cancellationToken)
        {
            var result = await _examService.GetTeacherCreateOptionsAsync(cancellationToken);
            return Ok(result);
        }

        // YENI
        // Teacher-in öz imtahanlarını filter ilə gətirir
        [HttpGet("teacher/my")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetMyExams(
            [FromQuery] int? classRoomId,
            [FromQuery] int? subjectId,
            [FromQuery] bool? isPublished,
            [FromQuery] string? status,
            CancellationToken cancellationToken)
        {
            var filter = new TeacherExamListFilterDto
            {
                ClassRoomId = classRoomId,
                SubjectId = subjectId,
                IsPublished = isPublished,
                Status = status
            };

            var result = await _examService.GetMyExamsAsync(filter, cancellationToken);
            return Ok(result);
        }

        // YENI
        // Müəyyən imtahan üzrə submit edən tələbələrin siyahısını gətirir
        [HttpGet("{examId:int}/submissions")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetExamSubmissions(
            [FromRoute] int examId,
            CancellationToken cancellationToken)
        {
            var result = await _examService.GetExamSubmissionsAsync(examId, cancellationToken);
            return Ok(result);
        }

        // YENI
        // Müəyyən tələbənin həmin imtahan üzrə submission detail-i
        [HttpGet("{examId:int}/submissions/{studentExamId:int}")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GetStudentExamSubmissionDetail(
            [FromRoute] int examId,
            [FromRoute] int studentExamId,
            CancellationToken cancellationToken)
        {
            var result = await _examService.GetStudentExamSubmissionDetailAsync(
                examId,
                studentExamId,
                cancellationToken);

            return Ok(result);
        }

        // YENI
        // Teacher açıq sualları qiymətləndirir
        [HttpPost("submissions/grade")]
        [Authorize(Roles = "Admin,IsSuperAdmin,Teacher")]
        public async Task<IActionResult> GradeStudentExam(
            [FromBody] GradeStudentExamRequestDto request,
            CancellationToken cancellationToken)
        {
            var result = await _examService.GradeStudentExamAsync(request, cancellationToken);
            return Ok(result);
        }
    }
}