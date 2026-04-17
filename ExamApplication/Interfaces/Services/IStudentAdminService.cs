using ExamApplication.DTO.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    public interface IStudentAdminService
    {
        Task<List<StudentListItemDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<StudentDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<StudentDetailDto> CreateAsync(CreateStudentDto request, CancellationToken cancellationToken = default);

        Task<StudentDetailDto> UpdateAsync(UpdateStudentDto request, CancellationToken cancellationToken = default);

        Task DeleteAsync(int id, CancellationToken cancellationToken = default);

        Task<List<StudentOptionDto>> SearchAsync(string? search, CancellationToken cancellationToken = default);
        Task<StudentAttendanceSummaryDto> UpdateAttendanceAsync(
    int studentId,
    UpdateStudentAttendanceRecordDto request,
    CancellationToken cancellationToken = default
);
        Task<List<StudentOptionDto>> GetOptionsAsync(CancellationToken cancellationToken = default);

        Task<List<StudentTaskDto>> GetTasksAsync(int studentId, CancellationToken cancellationToken = default);

        Task<StudentTaskDto> CreateTaskAsync(int studentId, CreateStudentTaskDto request, CancellationToken cancellationToken = default);

        Task<StudentTaskDto> UpdateTaskAsync(int studentId, UpdateStudentTaskDto request, CancellationToken cancellationToken = default);

        Task DeleteTaskAsync(int studentId, int taskId, CancellationToken cancellationToken = default);

        Task<StudentExamReviewDto> GetExamReviewAsync(int studentId, int studentExamId, CancellationToken cancellationToken = default);
    }
}
