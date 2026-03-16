using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface IExamOptionRepository
    {
        Task<ExamOption?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<List<ExamOption>> GetByQuestionIdAsync(int questionId, CancellationToken cancellationToken = default);

        Task AddAsync(ExamOption option, CancellationToken cancellationToken = default);

        void Update(ExamOption option);

        void Remove(ExamOption option);
    }
}
