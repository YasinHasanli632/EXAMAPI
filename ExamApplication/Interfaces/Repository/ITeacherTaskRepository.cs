using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface ITeacherTaskRepository
    {
        Task<TeacherTask?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<List<TeacherTask>> GetByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default);

        Task<List<TeacherTask>> GetPendingByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default);

        Task AddAsync(TeacherTask teacherTask, CancellationToken cancellationToken = default);

        void Update(TeacherTask teacherTask);

        void Remove(TeacherTask teacherTask);
    }
}
