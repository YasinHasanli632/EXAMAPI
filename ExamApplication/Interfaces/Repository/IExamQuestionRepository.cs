using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface IExamQuestionRepository
    {
        Task<ExamQuestion?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<ExamQuestion?> GetByIdWithOptionsAsync(int id, CancellationToken cancellationToken = default);

        Task<List<ExamQuestion>> GetByExamIdAsync(int examId, CancellationToken cancellationToken = default);

        Task AddAsync(ExamQuestion question, CancellationToken cancellationToken = default);

        void Update(ExamQuestion question);

        void Remove(ExamQuestion question);
    }
}
