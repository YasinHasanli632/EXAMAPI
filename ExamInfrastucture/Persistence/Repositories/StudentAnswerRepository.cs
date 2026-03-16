using ExamApplication.Interfaces.Repository;
using ExamDomain.Entities;
using ExamDomain.Enum;
using ExamInfrastucture.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamInfrastucture.Persistence.Repositories
{
    public class StudentAnswerRepository : IStudentAnswerRepository
    {
        private readonly AppDbContext _context;

        public StudentAnswerRepository(AppDbContext context)
        {
            _context = context;
        }

        // Id-yə görə student answer gətirir
        public async Task<StudentAnswer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.StudentAnswers
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // Student session və sual cütünə görə cavabı gətirir
        public async Task<StudentAnswer?> GetByStudentExamAndQuestionAsync(int studentExamId, int questionId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentAnswers
                .FirstOrDefaultAsync(x => x.StudentExamId == studentExamId && x.ExamQuestionId == questionId, cancellationToken);
        }

        // Student session-a aid bütün cavabları gətirir
        public async Task<List<StudentAnswer>> GetByStudentExamIdAsync(int studentExamId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentAnswers
                .Include(x => x.ExamQuestion)
                .Where(x => x.StudentExamId == studentExamId)
                .OrderBy(x => x.ExamQuestionId)
                .ToListAsync(cancellationToken);
        }

        // Verilən imtahandakı açıq tip cavabları gətirir
        public async Task<List<StudentAnswer>> GetOpenAnswersByExamIdAsync(int examId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentAnswers
                .Include(x => x.StudentExam)
                    .ThenInclude(x => x.Student)
                .Include(x => x.ExamQuestion)
                .Where(x => x.StudentExam.ExamId == examId &&
                            x.ExamQuestion.QuestionType == QuestionType.OpenText)
                .OrderBy(x => x.StudentExam.Student.FullName)
                .ToListAsync(cancellationToken);
        }

        // Yeni student answer əlavə edir
        public async Task AddAsync(StudentAnswer studentAnswer, CancellationToken cancellationToken = default)
        {
            await _context.StudentAnswers.AddAsync(studentAnswer, cancellationToken);
        }

        // Student answer-i update üçün context-ə işarələyir
        public void Update(StudentAnswer studentAnswer)
        {
            _context.StudentAnswers.Update(studentAnswer);
        }

        // Student answer-i silmək üçün context-ə işarələyir
        public void Remove(StudentAnswer studentAnswer)
        {
            _context.StudentAnswers.Remove(studentAnswer);
        }
    }
}
