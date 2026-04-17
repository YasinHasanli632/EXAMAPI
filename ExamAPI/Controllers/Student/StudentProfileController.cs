using ExamApplication.DTO.Student;
using ExamApplication.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExamAPI.Controllers.Student
{
    [Route("api/student/profile")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class StudentProfileController : ControllerBase
    {
        private readonly IStudentProfileService _studentProfileService;

        public StudentProfileController(IStudentProfileService studentProfileService)
        {
            _studentProfileService = studentProfileService;
        }

        // YENI
        [HttpGet]
        public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
        {
            var result = await _studentProfileService.GetMyProfileAsync(cancellationToken);
            return Ok(result);
        }

        // YENI
        [HttpPut]
        public async Task<IActionResult> UpdateMyProfile(
            [FromBody] UpdateMyStudentProfileDto request,
            CancellationToken cancellationToken)
        {
            var result = await _studentProfileService.UpdateMyProfileAsync(request, cancellationToken);
            return Ok(result);
        }
    }
}
