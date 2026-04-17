using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface IClassRoomRepository
    {
        Task<ClassRoom?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<ClassRoom?> GetByIdWithStudentsAsync(int id, CancellationToken cancellationToken = default);

        Task<ClassRoom?> GetByIdWithTeacherAssignmentsAsync(int id, CancellationToken cancellationToken = default);

        Task<ClassRoom?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);

        Task<List<ClassRoom>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<List<ClassRoom>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);

        Task<List<ClassRoom>> GetByAcademicYearAsync(string academicYear, CancellationToken cancellationToken = default); // YENI

        Task<List<ClassRoom>> GetActiveAsync(CancellationToken cancellationToken = default); // YENI

        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameAsync(string name, int excludeId, CancellationToken cancellationToken = default);

        Task AddAsync(ClassRoom classRoom, CancellationToken cancellationToken = default);

        void Update(ClassRoom classRoom);

        void Remove(ClassRoom classRoom);

        Task<List<ClassRoom>> GetByIdsAsync(List<int> classRoomIds, CancellationToken cancellationToken = default);
    }
}
