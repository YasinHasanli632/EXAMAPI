using ExamApplication.DTO.Teacher.Task;
using ExamApplication.DTO.Teacher.Task.ExamApplication.DTO.Teacher.Tasks;
using ExamApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExamAPI.Controllers.TeacherTasks
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Teacher")]
    public class TeacherTasksController : ControllerBase
    {
        private readonly ITeacherTaskManagementService _teacherTaskManagementService;

        public TeacherTasksController(ITeacherTaskManagementService teacherTaskManagementService)
        {
            _teacherTaskManagementService = teacherTaskManagementService;
        }

        /// <summary>
        /// Müəllimin task idarə edə biləcəyi sinifləri gətirir
        /// </summary>
        [HttpGet("classes")]
        [ProducesResponseType(typeof(List<TeacherTaskClassSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyTaskClasses(CancellationToken cancellationToken)
        {
            var result = await _teacherTaskManagementService.GetMyTaskClassesAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Müəyyən sinif üzrə müəllimin bütün ümumi tasklarını gətirir
        /// </summary>
        [HttpGet("classes/{classRoomId:int}/tasks")]
        [ProducesResponseType(typeof(List<TeacherClassTaskListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClassTasks([FromRoute] int classRoomId, CancellationToken cancellationToken)
        {
            var result = await _teacherTaskManagementService.GetClassTasksAsync(classRoomId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Sinif üçün yeni task yaradır və həmin sinifdəki bütün şagirdlər üçün StudentTask record yaradır
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(TeacherTaskDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateClassTask([FromBody] CreateTeacherClassTaskDto request, CancellationToken cancellationToken)
        {
            var result = await _teacherTaskManagementService.CreateClassTaskAsync(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// TaskGroupKey üzrə task detailini gətirir
        /// </summary>
        [HttpGet("{taskGroupKey}")]
        [ProducesResponseType(typeof(TeacherTaskDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTaskDetail([FromRoute] string taskGroupKey, CancellationToken cancellationToken)
        {
            var result = await _teacherTaskManagementService.GetTaskDetailAsync(taskGroupKey, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Vaxtı bitməyən taskı yeniləyir
        /// </summary>
        [HttpPut("{taskGroupKey}")]
        [ProducesResponseType(typeof(TeacherTaskDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateClassTask(
            [FromRoute] string taskGroupKey,
            [FromBody] UpdateTeacherClassTaskDto request,
            CancellationToken cancellationToken)
        {
            request.TaskGroupKey = taskGroupKey;
            var result = await _teacherTaskManagementService.UpdateClassTaskAsync(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// TaskGroupKey üzrə həmin taskın bütün student task recordlarını silir
        /// </summary>
        [HttpDelete("{taskGroupKey}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteClassTask([FromRoute] string taskGroupKey, CancellationToken cancellationToken)
        {
            await _teacherTaskManagementService.DeleteClassTaskAsync(taskGroupKey, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Müəyyən task üzrə konkret şagirdin submission detailini gətirir
        /// </summary>
        [HttpGet("{taskGroupKey}/submissions/{studentTaskId:int}")]
        [ProducesResponseType(typeof(StudentTaskSubmissionDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudentTaskSubmissionDetail(
            [FromRoute] string taskGroupKey,
            [FromRoute] int studentTaskId,
            CancellationToken cancellationToken)
        {
            var result = await _teacherTaskManagementService
                .GetStudentTaskSubmissionDetailAsync(taskGroupKey, studentTaskId, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Müəllim şagird taskını qiymətləndirir
        /// </summary>
        [HttpPut("submissions/grade")]
        [ProducesResponseType(typeof(StudentTaskSubmissionDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GradeStudentTask([FromBody] GradeStudentTaskDto request, CancellationToken cancellationToken)
        {
            var result = await _teacherTaskManagementService.GradeStudentTaskAsync(request, cancellationToken);
            return Ok(result);
        }
    }
}
