using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
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
            // YENI
            Task<StudentExam?> GetByIdWithFullDetailsAsync(int id, CancellationToken cancellationToken = default);

            // YENI
            Task<StudentExam?> GetByIdAndStudentAsync(int id, int studentId, CancellationToken cancellationToken = default);

            // YENI
            Task<StudentExam?> GetByIdAndStudentWithFullDetailsAsync(int id, int studentId, CancellationToken cancellationToken = default);

            // YENI
            Task<List<StudentExam>> GetCompletedByExamIdWithDetailsAsync(int examId, CancellationToken cancellationToken = default);

            Task<StudentExam?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

            Task<StudentExam?> GetByIdWithAnswersAsync(int id, CancellationToken cancellationToken = default);

            Task<StudentExam?> GetByStudentAndExamAsync(int studentId, int examId, CancellationToken cancellationToken = default);

            Task<List<StudentExam>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);

            Task<List<StudentExam>> GetByExamIdAsync(int examId, CancellationToken cancellationToken = default);

            Task<List<StudentExam>> GetCompletedByExamIdAsync(int examId, CancellationToken cancellationToken = default);

            Task<bool> ExistsAsync(int studentId, int examId, CancellationToken cancellationToken = default);

            Task AddAsync(StudentExam studentExam, CancellationToken cancellationToken = default);

            // YENI
            Task<List<StudentExam>> GetByStudentIdWithDetailsAsync(int studentId, CancellationToken cancellationToken = default);

            // YENI
            Task<StudentExam?> GetByStudentAndExamWithFullDetailsAsync(int studentId, int examId, CancellationToken cancellationToken = default);

            // YENI
            Task<List<StudentExam>> GetPendingByExamIdAsync(int examId, CancellationToken cancellationToken = default);

            // YENI
            Task<List<StudentExam>> GetNotCompletedActiveSessionsAsync(CancellationToken cancellationToken = default);

            void Update(StudentExam studentExam);

            void Remove(StudentExam studentExam);
        }
    }
}
