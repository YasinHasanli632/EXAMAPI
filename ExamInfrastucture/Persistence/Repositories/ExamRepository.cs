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
    public class ExamRepository : IExamRepository
    {
        private readonly AppDbContext _context;

        public ExamRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Exam?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Exam?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(x => x.ClassRoom)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Include(x => x.Questions.OrderBy(q => q.OrderNumber))
                    .ThenInclude(x => x.Options.OrderBy(o => o.OrderNumber))
                .Include(x => x.StudentExams)
                    .ThenInclude(x => x.Student)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<Exam>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(x => x.ClassRoom)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .OrderByDescending(x => x.StartTime)
                .ToListAsync(cancellationToken);
        }

        // YENI
        public async Task<List<Exam>> GetPublishedUpcomingExamsAsync(DateTime now, DateTime until, CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(x => x.StudentExams)
                .Where(x => x.IsPublished && x.StartTime > now && x.StartTime <= until)
                .ToListAsync(cancellationToken);
        }

        // YENI
        public async Task<List<Exam>> GetPublishedActiveExamsAsync(DateTime now, CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(x => x.StudentExams)
                .Where(x => x.IsPublished && x.StartTime <= now && x.EndTime >= now)
                .ToListAsync(cancellationToken);
        }

        // YENI
        public async Task<List<Exam>> GetPublishedEndedExamsAsync(DateTime now, CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(x => x.StudentExams)
                .Where(x => x.IsPublished && x.EndTime < now)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Exam>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(x => x.ClassRoom)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Include(x => x.Questions)
                    .ThenInclude(x => x.Options)
                .Include(x => x.StudentExams)
                .OrderByDescending(x => x.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Exam>> GetByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(x => x.ClassRoom)
                .Include(x => x.Subject)
                .Where(x => x.TeacherId == teacherId)
                .OrderByDescending(x => x.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Exam>> GetBySubjectIdAsync(int subjectId, CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(x => x.ClassRoom)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Where(x => x.SubjectId == subjectId)
                .OrderByDescending(x => x.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Exam>> GetByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Where(x => x.ClassRoomId == classRoomId)
                .OrderByDescending(x => x.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Exam>> GetActiveExamsAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            return await _context.Exams
                .Include(x => x.ClassRoom)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                // YENI
                // Status köhnə data-da fərqli olsa belə aktivlik publish və tarixə görə hesablanır
                .Where(x => x.IsPublished && x.StartTime <= now && x.EndTime > now && x.Status != ExamStatus.Cancelled)
                .OrderBy(x => x.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Exam>> GetPublishedByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Where(x => x.ClassRoomId == classRoomId && x.IsPublished)
                .OrderByDescending(x => x.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Exam exam, CancellationToken cancellationToken = default)
        {
            await _context.Exams.AddAsync(exam, cancellationToken);
        }

        public void Update(Exam exam)
        {
            _context.Exams.Update(exam);
        }

        public void Remove(Exam exam)
        {
            _context.Exams.Remove(exam);
        }

        // YENI
        public async Task<List<Exam>> GetByTeacherIdsAsync(List<int> teacherIds, CancellationToken cancellationToken = default)
        {
            if (teacherIds == null || teacherIds.Count == 0)
                return new List<Exam>();

            return await _context.Exams
                .Include(x => x.ClassRoom)
                .Include(x => x.Subject)
                .Where(x => teacherIds.Contains(x.TeacherId))
                .OrderByDescending(x => x.StartTime)
                .ToListAsync(cancellationToken);
        }

        // YENI
        public async Task<List<Exam>> GetByTeacherIdWithFullStatsAsync(int teacherId, CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(x => x.ClassRoom)
                .Include(x => x.Subject)
                .Include(x => x.Questions)
                .Include(x => x.StudentExams)
                .Where(x => x.TeacherId == teacherId)
                .OrderByDescending(x => x.StartTime)
                .ToListAsync(cancellationToken);
        }
    }
}
