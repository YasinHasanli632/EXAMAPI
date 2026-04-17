using ExamApplication.DTO.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    // YENI - SAGIRD UCUN
    public interface IStudentTaskService
    {
        Task<List<StudentTaskListItemDto>> GetMyTasksAsync(int? subjectId, CancellationToken cancellationToken = default);

        Task<StudentTaskDetailDto> GetMyTaskDetailAsync(int studentTaskId, CancellationToken cancellationToken = default);

        Task<StudentTaskDetailDto> SubmitMyTaskAsync(int studentTaskId, SubmitStudentTaskDto request, CancellationToken cancellationToken = default);

        Task<StudentTaskSummaryDto> GetMyTaskSummaryAsync(CancellationToken cancellationToken = default);
    }
}
