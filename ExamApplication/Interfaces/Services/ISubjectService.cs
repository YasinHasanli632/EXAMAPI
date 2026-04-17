using ExamApplication.DTO.Subject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    public interface ISubjectService
    {
        Task<SubjectDto> CreateAsync(CreateSubjectDto request, CancellationToken cancellationToken = default);

        Task<SubjectDto> UpdateAsync(UpdateSubjectDto request, CancellationToken cancellationToken = default);

        Task<SubjectDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<SubjectDetailsDto> GetDetailsByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<List<SubjectDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<List<SubjectDetailsDto>> GetAllDetailsAsync(CancellationToken cancellationToken = default);

        Task DeleteAsync(int id, CancellationToken cancellationToken = default);

        Task ChangeStatusAsync(ChangeSubjectStatusDto request, CancellationToken cancellationToken = default);

        Task SyncTeachersAsync(SyncSubjectTeachersDto request, CancellationToken cancellationToken = default);

        Task<List<SubjectTeacherDto>> GetTeachersBySubjectIdAsync(int subjectId, CancellationToken cancellationToken = default);
    }
}
