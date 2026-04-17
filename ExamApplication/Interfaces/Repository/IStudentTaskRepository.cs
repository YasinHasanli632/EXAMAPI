using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface IStudentTaskRepository
    {
        Task<StudentTask?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<List<StudentTask>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);

        Task AddAsync(StudentTask studentTask, CancellationToken cancellationToken = default);

        // YENI
        Task AddRangeAsync(List<StudentTask> studentTasks, CancellationToken cancellationToken = default);

        void Update(StudentTask studentTask);

        // YENI
        void UpdateRange(List<StudentTask> studentTasks);

        void Remove(StudentTask studentTask);

        // YENI
        void RemoveRange(List<StudentTask> studentTasks);

        Task<List<StudentTask>> GetByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default);

        Task<List<StudentTask>> GetByTeacherAndStudentIdsAsync(int teacherId, List<int> studentIds, CancellationToken cancellationToken = default);

        // YENI
        Task<List<StudentTask>> GetByTeacherAndClassRoomIdAsync(int teacherId, int classRoomId, CancellationToken cancellationToken = default);
        // YENI - SAGIRD UCUN
        Task<List<StudentTask>> GetActiveByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);

        // YENI - SAGIRD UCUN
        Task<List<StudentTask>> GetByStudentIdAndSubjectIdAsync(int studentId, int subjectId, CancellationToken cancellationToken = default);

        // YENI - SAGIRD UCUN
        Task<StudentTask?> GetStudentTaskDetailAsync(int studentTaskId, int studentId, CancellationToken cancellationToken = default);

        // YENI - SAGIRD UCUN
        Task<List<StudentTask>> GetRecentByStudentIdAsync(int studentId, int take = 5, CancellationToken cancellationToken = default);
        // YENI
        Task<List<StudentTask>> GetByTaskGroupKeyAsync(string taskGroupKey, CancellationToken cancellationToken = default);

        // YENI
        Task<StudentTask?> GetByTaskGroupKeyAndStudentTaskIdAsync(string taskGroupKey, int studentTaskId, CancellationToken cancellationToken = default);

        // YENI
        Task<bool> ExistsTaskGroupAsync(string taskGroupKey, CancellationToken cancellationToken = default);
    }
}
