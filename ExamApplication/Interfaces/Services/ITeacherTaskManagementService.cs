using ExamApplication.DTO.Teacher.Task;
using ExamApplication.DTO.Teacher.Task.ExamApplication.DTO.Teacher.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    public interface ITeacherTaskManagementService
    {
        Task<List<TeacherTaskClassSummaryDto>> GetMyTaskClassesAsync(CancellationToken cancellationToken = default);

        Task<List<TeacherClassTaskListItemDto>> GetClassTasksAsync(int classRoomId, CancellationToken cancellationToken = default);

        Task<TeacherTaskDetailDto> GetTaskDetailAsync(string taskGroupKey, CancellationToken cancellationToken = default);

        Task<TeacherTaskDetailDto> CreateClassTaskAsync(CreateTeacherClassTaskDto request, CancellationToken cancellationToken = default);

        Task<TeacherTaskDetailDto> UpdateClassTaskAsync(UpdateTeacherClassTaskDto request, CancellationToken cancellationToken = default);

        Task DeleteClassTaskAsync(string taskGroupKey, CancellationToken cancellationToken = default);

        Task<StudentTaskSubmissionDetailDto> GetStudentTaskSubmissionDetailAsync(string taskGroupKey, int studentTaskId, CancellationToken cancellationToken = default);

        Task<StudentTaskSubmissionDetailDto> GradeStudentTaskAsync(GradeStudentTaskDto request, CancellationToken cancellationToken = default);
    }
}
