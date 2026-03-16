using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface IStudentRepository
    {
        Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<Student?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

        Task<Student?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);

        Task<List<Student>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<List<Student>> GetStudentsByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default);

        Task<bool> ExistsByStudentNumberAsync(string studentNumber, CancellationToken cancellationToken = default);

        Task AddAsync(Student student, CancellationToken cancellationToken = default);

        void Update(Student student);

        void Remove(Student student);
    }
}
