using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface ITeacherRepository
    {
        Task<Teacher?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Teacher?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<Teacher?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
        Task<Teacher?> GetByUserIdWithDetailsAsync(int userId, CancellationToken cancellationToken = default);
        Task<List<Teacher>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<Teacher>> GetAllWithSubjectsAsync(CancellationToken cancellationToken = default);
        Task<List<Teacher>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Teacher teacher, CancellationToken cancellationToken = default);
        void Update(Teacher teacher);
        void Remove(Teacher teacher);

        // YENI
        // Teacher detail page üçün ən full include-lu obyekt
        Task<Teacher?> GetByIdWithFullDashboardAsync(int id, CancellationToken cancellationToken = default);

        // YENI
        Task<Teacher?> GetByUserIdWithFullDashboardAsync(int userId, CancellationToken cancellationToken = default);

        // YENI
        // Edit və batch mapping zamanı faydalıdır
        Task<List<Teacher>> GetByIdsWithDetailsAsync(List<int> teacherIds, CancellationToken cancellationToken = default);

        // YENI
        Task<bool> ExistsByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    }
}
