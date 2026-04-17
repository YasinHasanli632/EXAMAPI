using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface IStudentAnswerRepository
    {
        Task<StudentAnswer?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<StudentAnswer?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default); // YENI
                                                                                                             // YENI
        Task<List<StudentAnswer>> GetByStudentExamIdForTeacherReviewAsync(int studentExamId, CancellationToken cancellationToken = default);

        // YENI
        Task<List<StudentAnswer>> GetOpenAnswersByStudentExamIdAsync(int studentExamId, CancellationToken cancellationToken = default);
        Task<StudentAnswer?> GetByStudentExamAndQuestionAsync(int studentExamId, int questionId, CancellationToken cancellationToken = default);

        Task<List<StudentAnswer>> GetByStudentExamIdAsync(int studentExamId, CancellationToken cancellationToken = default);

        Task<List<StudentAnswer>> GetByStudentExamIdWithDetailsAsync(int studentExamId, CancellationToken cancellationToken = default); // YENI

        Task<List<StudentAnswer>> GetOpenAnswersByExamIdAsync(int examId, CancellationToken cancellationToken = default);

        Task AddAsync(StudentAnswer studentAnswer, CancellationToken cancellationToken = default);

        void Update(StudentAnswer studentAnswer);

        void Remove(StudentAnswer studentAnswer);
    }
}
