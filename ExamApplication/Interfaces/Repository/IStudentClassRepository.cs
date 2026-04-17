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

        Task<List<StudentClass>> GetActiveByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default); // YENI

        Task<List<StudentClass>> GetActiveByStudentIdAsync(int studentId, CancellationToken cancellationToken = default); // YENI

        Task<List<StudentClass>> GetByStudentIdsAsync(List<int> studentIds, CancellationToken cancellationToken = default); // YENI

        Task<bool> ExistsAsync(int studentId, int classRoomId, CancellationToken cancellationToken = default);
        // YENI
        Task<bool> AreAllStudentsActiveInClassAsync(
            int classRoomId,
            List<int> studentIds,
            CancellationToken cancellationToken = default);
        Task AddAsync(StudentClass studentClass, CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<StudentClass> studentClasses, CancellationToken cancellationToken = default); // YENI

        void Update(StudentClass studentClass);

        void Remove(StudentClass studentClass);

        void RemoveRange(IEnumerable<StudentClass> studentClasses); // YENI
    }
}
