using ExamApplication.Interfaces.Repository;
using ExamDomain.Entities;
using ExamInfrastucture.DAL;
using Microsoft.EntityFrameworkCore;

namespace ExamInfrastucture.Persistence.Repositories
{
    public class StudentTaskRepository : IStudentTaskRepository
    {
        private readonly AppDbContext _context;

        public StudentTaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StudentTask?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.StudentTasks
                .AsNoTracking()
                .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Include(x => x.ClassRoom)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<StudentTask>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentTasks
                .AsNoTracking()
                .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Include(x => x.ClassRoom)
                .Where(x => x.StudentId == studentId)
                .OrderByDescending(x => x.AssignedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<StudentTask>> GetByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentTasks
                .AsNoTracking()
                .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Include(x => x.ClassRoom)
                .Where(x => x.TeacherId == teacherId)
                .OrderByDescending(x => x.DueDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<StudentTask>> GetByTeacherAndStudentIdsAsync(int teacherId, List<int> studentIds, CancellationToken cancellationToken = default)
        {
            return await _context.StudentTasks
                .AsNoTracking()
                .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Include(x => x.ClassRoom)
                .Where(x => x.TeacherId == teacherId && studentIds.Contains(x.StudentId))
                .OrderByDescending(x => x.DueDate)
                .ToListAsync(cancellationToken);
        }

        // YENI
        public async Task<List<StudentTask>> GetByTeacherAndClassRoomIdAsync(int teacherId, int classRoomId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentTasks
                .AsNoTracking()
                .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Include(x => x.ClassRoom)
                .Where(x => x.TeacherId == teacherId && x.ClassRoomId == classRoomId)
                .OrderByDescending(x => x.AssignedDate)
                .ToListAsync(cancellationToken);
        }

        // YENI - SAGIRD UCUN
        public async Task<List<StudentTask>> GetActiveByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentTasks
                .AsNoTracking()
                .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Include(x => x.ClassRoom)
                .Where(x => x.StudentId == studentId && x.IsActive)
                .OrderByDescending(x => x.AssignedDate)
                .ToListAsync(cancellationToken);
        }

        // YENI - SAGIRD UCUN
        public async Task<List<StudentTask>> GetByStudentIdAndSubjectIdAsync(int studentId, int subjectId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentTasks
                .AsNoTracking()
                .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Include(x => x.ClassRoom)
                .Where(x => x.StudentId == studentId && x.SubjectId == subjectId && x.IsActive)
                .OrderByDescending(x => x.AssignedDate)
                .ToListAsync(cancellationToken);
        }

        // YENI - SAGIRD UCUN
        public async Task<StudentTask?> GetStudentTaskDetailAsync(int studentTaskId, int studentId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentTasks
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Include(x => x.ClassRoom)
                .FirstOrDefaultAsync(
                    x => x.Id == studentTaskId && x.StudentId == studentId && x.IsActive,
                    cancellationToken);
        }

        // YENI - SAGIRD UCUN
        public async Task<List<StudentTask>> GetRecentByStudentIdAsync(int studentId, int take = 5, CancellationToken cancellationToken = default)
        {
            return await _context.StudentTasks
                .AsNoTracking()
                .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Include(x => x.ClassRoom)
                .Where(x => x.StudentId == studentId && x.IsActive)
                .OrderByDescending(x => x.AssignedDate)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<StudentTask>> GetByTaskGroupKeyAsync(string taskGroupKey, CancellationToken cancellationToken = default)
        {
            return await _context.StudentTasks
                .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Include(x => x.ClassRoom)
                .Where(x => x.TaskGroupKey == taskGroupKey)
                .OrderBy(x => x.Student.FullName)
                .ToListAsync(cancellationToken);
        }

        // YENI
        public async Task<StudentTask?> GetByTaskGroupKeyAndStudentTaskIdAsync(string taskGroupKey, int studentTaskId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentTasks
                .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Include(x => x.ClassRoom)
                .FirstOrDefaultAsync(x => x.TaskGroupKey == taskGroupKey && x.Id == studentTaskId, cancellationToken);
        }

        // YENI
        public async Task<bool> ExistsTaskGroupAsync(string taskGroupKey, CancellationToken cancellationToken = default)
        {
            return await _context.StudentTasks
                .AnyAsync(x => x.TaskGroupKey == taskGroupKey, cancellationToken);
        }

        public async Task AddAsync(StudentTask studentTask, CancellationToken cancellationToken = default)
        {
            await _context.StudentTasks.AddAsync(studentTask, cancellationToken);
        }

        // YENI
        public async Task AddRangeAsync(List<StudentTask> studentTasks, CancellationToken cancellationToken = default)
        {
            await _context.StudentTasks.AddRangeAsync(studentTasks, cancellationToken);
        }

        public void Update(StudentTask studentTask)
        {
            _context.StudentTasks.Update(studentTask);
        }

        // YENI
        public void UpdateRange(List<StudentTask> studentTasks)
        {
            _context.StudentTasks.UpdateRange(studentTasks);
        }

        public void Remove(StudentTask studentTask)
        {
            _context.StudentTasks.Remove(studentTask);
        }

        // YENI
        public void RemoveRange(List<StudentTask> studentTasks)
        {
            _context.StudentTasks.RemoveRange(studentTasks);
        }
    }
}