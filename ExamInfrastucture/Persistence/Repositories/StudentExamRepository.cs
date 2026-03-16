using ExamApplication.Interfaces.Repository;
using ExamDomain.Entities;
using ExamInfrastucture.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamInfrastucture.Persistence.Repositories
{
    public class StudentExamRepository : IStudentExamRepository
    {
        private readonly AppDbContext _context;

        public StudentExamRepository(AppDbContext context)
        {
            _context = context;
        }

        // Id-yə görə student exam session-u gətirir
        public async Task<StudentExam?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.StudentExams
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // Id-yə görə student exam session-u cavabları ilə gətirir
        public async Task<StudentExam?> GetByIdWithAnswersAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.StudentExams
                .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                .Include(x => x.Exam)
                    .ThenInclude(x => x.Subject)
                .Include(x => x.Answers)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // Student və exam-a görə session-u gətirir
        public async Task<StudentExam?> GetByStudentAndExamAsync(int studentId, int examId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentExams
                .FirstOrDefaultAsync(x => x.StudentId == studentId && x.ExamId == examId, cancellationToken);
        }

        // Student-in bütün exam session-larını gətirir
        public async Task<List<StudentExam>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentExams
                .Include(x => x.Exam)
                    .ThenInclude(x => x.Subject)
                .Where(x => x.StudentId == studentId)
                .OrderByDescending(x => x.StartTime)
                .ToListAsync(cancellationToken);
        }

        // İmtahana aid bütün student session-larını gətirir
        public async Task<List<StudentExam>> GetByExamIdAsync(int examId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentExams
                .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                .Where(x => x.ExamId == examId)
                .OrderByDescending(x => x.StartTime)
                .ToListAsync(cancellationToken);
        }

        // İmtahana aid tamamlanmış student session-larını gətirir
        public async Task<List<StudentExam>> GetCompletedByExamIdAsync(int examId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentExams
                .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                .Where(x => x.ExamId == examId && x.IsCompleted)
                .OrderByDescending(x => x.EndTime)
                .ToListAsync(cancellationToken);
        }

        // Student üçün həmin exam session-unun olub-olmadığını yoxlayır
        public async Task<bool> ExistsAsync(int studentId, int examId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentExams
                .AnyAsync(x => x.StudentId == studentId && x.ExamId == examId, cancellationToken);
        }

        // Yeni student exam session-u əlavə edir
        public async Task AddAsync(StudentExam studentExam, CancellationToken cancellationToken = default)
        {
            await _context.StudentExams.AddAsync(studentExam, cancellationToken);
        }

        // Student exam session-u update üçün context-ə işarələyir
        public void Update(StudentExam studentExam)
        {
            _context.StudentExams.Update(studentExam);
        }

        // Student exam session-u silmək üçün context-ə işarələyir
        public void Remove(StudentExam studentExam)
        {
            _context.StudentExams.Remove(studentExam);
        }
    }
}
