using ExamApplication.Interfaces.Repository;
using ExamDomain.Entities;
using ExamInfrastucture.DAL;
using Microsoft.EntityFrameworkCore;

namespace ExamInfrastucture.Persistence.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Student?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        }

        // YENI
        public async Task<List<Student>> GetByIdsWithDetailsAsync(List<int> ids, CancellationToken cancellationToken = default)
        {
            if (ids == null || ids.Count == 0)
                return new List<Student>();

            return await _context.Students
                .Include(x => x.User)
                .Include(x => x.StudentClasses)
                    .ThenInclude(x => x.ClassRoom)
                .Include(x => x.StudentExams)
                    .ThenInclude(x => x.Exam)
                        .ThenInclude(x => x.Subject)
                // YENI
                .Include(x => x.StudentExams)
                    .ThenInclude(x => x.Exam)
                        .ThenInclude(x => x.Teacher)
                .Include(x => x.AttendanceRecords)
                    .ThenInclude(x => x.AttendanceSession)
                        .ThenInclude(x => x.Subject)
                // YENI
                .Include(x => x.AttendanceRecords)
                    .ThenInclude(x => x.AttendanceSession)
                        .ThenInclude(x => x.Teacher)
                // YENI
                .Include(x => x.Tasks)
                    .ThenInclude(x => x.Subject)
                // YENI
                .Include(x => x.Tasks)
                    .ThenInclude(x => x.Teacher)
                .Where(x => ids.Contains(x.Id))
                .OrderBy(x => x.FullName)
                .ToListAsync(cancellationToken);
        }

        public async Task<Student?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .Include(x => x.User)
                .Include(x => x.StudentClasses)
                    .ThenInclude(x => x.ClassRoom)
                .Include(x => x.StudentExams)
                    .ThenInclude(x => x.Exam)
                        .ThenInclude(x => x.Subject)
                // YENI
                .Include(x => x.StudentExams)
                    .ThenInclude(x => x.Exam)
                        .ThenInclude(x => x.Teacher)
                .Include(x => x.AttendanceRecords)
                    .ThenInclude(x => x.AttendanceSession)
                        .ThenInclude(x => x.Subject)
                // YENI
                .Include(x => x.AttendanceRecords)
                    .ThenInclude(x => x.AttendanceSession)
                        .ThenInclude(x => x.Teacher)
                // YENI
                .Include(x => x.Tasks)
                    .ThenInclude(x => x.Subject)
                // YENI
                .Include(x => x.Tasks)
                    .ThenInclude(x => x.Teacher)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<Student>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .Include(x => x.User)
                .OrderBy(x => x.FullName)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Student>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .Include(x => x.User)
                .Include(x => x.StudentClasses)
                    .ThenInclude(x => x.ClassRoom)
                .Include(x => x.StudentExams)
                    .ThenInclude(x => x.Exam)
                        .ThenInclude(x => x.Subject)
                // YENI
                .Include(x => x.StudentExams)
                    .ThenInclude(x => x.Exam)
                        .ThenInclude(x => x.Teacher)
                .Include(x => x.AttendanceRecords)
                    .ThenInclude(x => x.AttendanceSession)
                        .ThenInclude(x => x.Subject)
                // YENI
                .Include(x => x.AttendanceRecords)
                    .ThenInclude(x => x.AttendanceSession)
                        .ThenInclude(x => x.Teacher)
                // YENI
                .Include(x => x.Tasks)
                    .ThenInclude(x => x.Subject)
                // YENI
                .Include(x => x.Tasks)
                    .ThenInclude(x => x.Teacher)
                .OrderBy(x => x.FullName)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Student>> GetStudentsByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentClasses
                .Where(sc => sc.ClassRoomId == classRoomId && sc.IsActive)
                .Include(sc => sc.Student)
                    .ThenInclude(s => s.User)
                .Include(sc => sc.Student)
                    .ThenInclude(s => s.StudentExams)
                        .ThenInclude(se => se.Exam)
                            .ThenInclude(e => e.Subject)
                // YENI
                .Include(sc => sc.Student)
                    .ThenInclude(s => s.StudentExams)
                        .ThenInclude(se => se.Exam)
                            .ThenInclude(e => e.Teacher)
                // YENI
                .Include(sc => sc.Student)
                    .ThenInclude(s => s.AttendanceRecords)
                        .ThenInclude(ar => ar.AttendanceSession)
                            .ThenInclude(a => a.Subject)
                // YENI
                .Include(sc => sc.Student)
                    .ThenInclude(s => s.AttendanceRecords)
                        .ThenInclude(ar => ar.AttendanceSession)
                            .ThenInclude(a => a.Teacher)
                // YENI
                .Include(sc => sc.Student)
                    .ThenInclude(s => s.Tasks)
                        .ThenInclude(t => t.Subject)
                // YENI
                .Include(sc => sc.Student)
                    .ThenInclude(s => s.Tasks)
                        .ThenInclude(t => t.Teacher)
                .Select(sc => sc.Student)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Student>> GetByIdsAsync(List<int> studentIds, CancellationToken cancellationToken = default)
        {
            if (studentIds == null || studentIds.Count == 0)
                return new List<Student>();

            return await _context.Students
                .Include(x => x.User)
                .Where(x => studentIds.Contains(x.Id))
                .OrderBy(x => x.FullName)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Student>> SearchByFullNameAsync(string search, CancellationToken cancellationToken = default)
        {
            search = search?.Trim() ?? string.Empty;

            return await _context.Students
                .Include(x => x.User)
                .Where(x =>
                    x.FullName.Contains(search) ||
                    x.User.FirstName.Contains(search) ||
                    x.User.LastName.Contains(search) ||
                    x.StudentNumber.Contains(search))
                .OrderBy(x => x.FullName)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByStudentNumberAsync(string studentNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .AnyAsync(x => x.StudentNumber == studentNumber, cancellationToken);
        }

        public async Task<bool> ExistsByStudentNumberAsync(string studentNumber, int excludeId, CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .AnyAsync(x => x.StudentNumber == studentNumber && x.Id != excludeId, cancellationToken);
        }

        public async Task AddAsync(Student student, CancellationToken cancellationToken = default)
        {
            await _context.Students.AddAsync(student, cancellationToken);
        }

        public void Update(Student student)
        {
            _context.Students.Update(student);
        }

        public void Remove(Student student)
        {
            _context.Students.Remove(student);
        }
    }
}