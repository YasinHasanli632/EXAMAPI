using ExamApplication.DTO.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    public interface IStudentExamService
    {
        Task<List<StudentExamSummaryDto>> GetMyExamsAsync(CancellationToken cancellationToken = default);

        // YENI
        Task<StudentExamDetailDto> GetMyExamDetailAsync(int examId, CancellationToken cancellationToken = default);

        // YENI
        Task<bool> VerifyAccessCodeAsync(VerifyStudentExamAccessCodeRequestDto request, CancellationToken cancellationToken = default);

        // YENI
        Task<StudentExamSessionDto> StartExamAsync(StartStudentExamRequestDto request, CancellationToken cancellationToken = default);

        // YENI
        Task<StudentAnswerDto> SaveAnswerAsync(SaveStudentAnswerRequestDto request, CancellationToken cancellationToken = default);

        // YENI
        Task<StudentExamSubmitResultDto> SubmitExamAsync(SubmitStudentExamRequestDto request, CancellationToken cancellationToken = default);

        // YENI
        // Student öz nəticə baxışını açsın
        Task<StudentExamReviewDto> GetMyExamReviewAsync(int studentExamId, CancellationToken cancellationToken = default);

        // YENI
        Task LogSecurityEventAsync(LogExamSecurityEventRequestDto request, CancellationToken cancellationToken = default);
    }
}
