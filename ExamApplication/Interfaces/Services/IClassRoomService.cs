using ExamApplication.DTO.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    public interface IClassRoomService
    {
        Task<List<ClassListItemDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<ClassDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<ClassDetailDto> CreateAsync(CreateClassDto request, CancellationToken cancellationToken = default);

        Task<ClassDetailDto> UpdateAsync(UpdateClassDto request, CancellationToken cancellationToken = default);

        Task DeleteAsync(int id, CancellationToken cancellationToken = default);

        Task<List<ClassStudentOptionDto>> SearchStudentsAsync(string? search, CancellationToken cancellationToken = default);

        Task<List<ClassTeacherOptionDto>> SearchTeachersAsync(string? search, CancellationToken cancellationToken = default);

        Task<List<ClassSubjectOptionDto>> GetSubjectOptionsAsync(CancellationToken cancellationToken = default);
    }
}
