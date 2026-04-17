using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface ISubjectRepository
    {
        Task<Subject?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // YENI
        Task<Subject?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);

        Task<List<Subject>> GetAllAsync(CancellationToken cancellationToken = default);

        // YENI
        Task<List<Subject>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);

        Task<List<Subject>> GetSubjectsByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

        // YENI
        Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);

        Task AddAsync(Subject subject, CancellationToken cancellationToken = default);

        void Update(Subject subject);

        void Remove(Subject subject);

        // YENI
        Task<List<Subject>> GetByIdsAsync(List<int> subjectIds, CancellationToken cancellationToken = default);
    }
}
