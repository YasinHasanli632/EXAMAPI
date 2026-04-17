using ExamApplication.DTO.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    public interface IStudentProfileService
    {
        // YENI
        Task<StudentDetailDto> GetMyProfileAsync(CancellationToken cancellationToken = default);

        // YENI
        Task<StudentDetailDto> UpdateMyProfileAsync(UpdateMyStudentProfileDto request, CancellationToken cancellationToken = default);
    }
}
