using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface IStudentExamRepository
    {
        Task<StudentExam?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<StudentExam?> GetByIdWithAnswersAsync(int id, CancellationToken cancellationToken = default);

        Task<StudentExam?> GetByStudentAndExamAsync(int studentId, int examId, CancellationToken cancellationToken = default);

        Task<List<StudentExam>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);

        Task<List<StudentExam>> GetByExamIdAsync(int examId, CancellationToken cancellationToken = default);

        Task<List<StudentExam>> GetCompletedByExamIdAsync(int examId, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(int studentId, int examId, CancellationToken cancellationToken = default);

        Task AddAsync(StudentExam studentExam, CancellationToken cancellationToken = default);

        void Update(StudentExam studentExam);

        void Remove(StudentExam studentExam);
    }
}
