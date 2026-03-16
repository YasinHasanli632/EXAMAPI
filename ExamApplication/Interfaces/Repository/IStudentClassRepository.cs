using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface IStudentClassRepository
    {
        Task<StudentClass?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<StudentClass?> GetByStudentAndClassAsync(int studentId, int classRoomId, CancellationToken cancellationToken = default);

        Task<List<StudentClass>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);

        Task<List<StudentClass>> GetByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(int studentId, int classRoomId, CancellationToken cancellationToken = default);

        Task AddAsync(StudentClass studentClass, CancellationToken cancellationToken = default);

        void Update(StudentClass studentClass);

        void Remove(StudentClass studentClass);
    }
}
