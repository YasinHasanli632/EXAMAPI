using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface IExamAccessCodeRepository
    {
        Task<ExamAccessCode?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<ExamAccessCode?> GetByCodeAsync(string accessCode, CancellationToken cancellationToken = default);

        Task<ExamAccessCode?> GetByExamAndStudentAsync(int examId, int studentId, CancellationToken cancellationToken = default);

        Task<List<ExamAccessCode>> GetByExamIdAsync(int examId, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(int examId, int studentId, CancellationToken cancellationToken = default);

        Task AddAsync(ExamAccessCode examAccessCode, CancellationToken cancellationToken = default);

        void Update(ExamAccessCode examAccessCode);

        void Remove(ExamAccessCode examAccessCode);
    }
}
