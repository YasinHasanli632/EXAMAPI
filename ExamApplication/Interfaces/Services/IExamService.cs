using ExamApplication.DTO.Exam;
using ExamApplication.DTO.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    public interface IExamService
    {
        Task<List<ExamListItemDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<ExamDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<ExamDetailDto> CreateAsync(CreateExamDto request, CancellationToken cancellationToken = default);

        Task<ExamDetailDto> UpdateAsync(UpdateExamDto request, CancellationToken cancellationToken = default);

        Task DeleteAsync(int id, CancellationToken cancellationToken = default);

        Task PublishAsync(int id, CancellationToken cancellationToken = default);

        Task<List<ExamClassOptionDto>> GetClassOptionsAsync(CancellationToken cancellationToken = default);

        Task<List<ExamSubjectOptionDto>> GetSubjectOptionsAsync(CancellationToken cancellationToken = default);

        Task<List<ExamTeacherOptionDto>> GetTeacherOptionsAsync(CancellationToken cancellationToken = default);

        // YENI
        Task EnsureStudentExamRecordsAsync(int examId, CancellationToken cancellationToken = default);

        // YENI
        Task<TeacherMyExamCreateOptionsDto> GetTeacherCreateOptionsAsync(CancellationToken cancellationToken = default);

        // YENI
        Task<List<ExamListItemDto>> GetMyExamsAsync(
            TeacherExamListFilterDto? filter = null,
            CancellationToken cancellationToken = default);

        // YENI
        Task<List<ExamSubmissionStudentDto>> GetExamSubmissionsAsync(
            int examId,
            CancellationToken cancellationToken = default);

        // YENI
        Task<ExamSubmissionDetailDto> GetStudentExamSubmissionDetailAsync(
            int examId,
            int studentExamId,
            CancellationToken cancellationToken = default);

        // YENI
        Task<GradeStudentExamResultDto> GradeStudentExamAsync(
            GradeStudentExamRequestDto request,
            CancellationToken cancellationToken = default);
    }
}
