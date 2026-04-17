using ExamApplication.DTO.Student;
using ExamApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExamAPI.Controllers.Student
{
    // YENI - SAGIRD UCUN
    [Route("api/student/tasks")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class StudentTasksController : ControllerBase
    {
        private readonly IStudentTaskService _studentTaskService;

        public StudentTasksController(IStudentTaskService studentTaskService)
        {
            _studentTaskService = studentTaskService;
        }

        // YENI - SAGIRD UCUN
        [HttpGet]
        [ProducesResponseType(typeof(List<StudentTaskListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyTasks([FromQuery] int? subjectId, CancellationToken cancellationToken)
        {
            var result = await _studentTaskService.GetMyTasksAsync(subjectId, cancellationToken);
            return Ok(result);
        }

        // YENI - SAGIRD UCUN
        [HttpGet("summary")]
        [ProducesResponseType(typeof(StudentTaskSummaryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyTaskSummary(CancellationToken cancellationToken)
        {
            var result = await _studentTaskService.GetMyTaskSummaryAsync(cancellationToken);
            return Ok(result);
        }

        // YENI - SAGIRD UCUN
        [HttpGet("{studentTaskId:int}")]
        [ProducesResponseType(typeof(StudentTaskDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyTaskDetail([FromRoute] int studentTaskId, CancellationToken cancellationToken)
        {
            var result = await _studentTaskService.GetMyTaskDetailAsync(studentTaskId, cancellationToken);
            return Ok(result);
        }

        // YENI - SAGIRD UCUN
        [HttpPost("{studentTaskId:int}/submit")]
        [ProducesResponseType(typeof(StudentTaskDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SubmitMyTask(
            [FromRoute] int studentTaskId,
            [FromBody] SubmitStudentTaskDto request,
            CancellationToken cancellationToken)
        {
            var result = await _studentTaskService.SubmitMyTaskAsync(studentTaskId, request, cancellationToken);
            return Ok(result);
        }
    }
}
