using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface IStudentAnswerOptionRepository
    {
        Task<List<StudentAnswerOption>> GetByStudentAnswerIdAsync(int studentAnswerId, CancellationToken cancellationToken = default);

        Task AddAsync(StudentAnswerOption entity, CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<StudentAnswerOption> entities, CancellationToken cancellationToken = default);

        void RemoveRange(IEnumerable<StudentAnswerOption> entities);
    }
}
