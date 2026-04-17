using ExamApplication.Interfaces.Repository;
using ExamDomain.Entities;
using ExamInfrastucture.DAL;
using Microsoft.EntityFrameworkCore;

namespace ExamInfrastucture.Persistence.Repositories
{
    public class AttendanceRecordRepository : IAttendanceRecordRepository
    {
        private readonly AppDbContext _context;

        public AttendanceRecordRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AttendanceRecord?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.AttendanceRecords
                .Include(x => x.Student)
                    .ThenInclude(s => s.User)
                .Include(x => x.AttendanceSession)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<AttendanceRecord>> GetBySessionIdAsync(int attendanceSessionId, CancellationToken cancellationToken = default)
        {
            return await _context.AttendanceRecords
                .Include(x => x.Student)
                    .ThenInclude(s => s.User)
                .Where(x => x.AttendanceSessionId == attendanceSessionId)
                .OrderBy(x => x.Student.FullName)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<AttendanceRecord>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
        {
            return await _context.AttendanceRecords
                .Include(x => x.AttendanceSession)
                    .ThenInclude(s => s.ClassRoom)
                .Include(x => x.AttendanceSession)
                    .ThenInclude(s => s.Subject)
                .Include(x => x.AttendanceSession)
                    .ThenInclude(s => s.Teacher)
                .Where(x => x.StudentId == studentId)
                .OrderByDescending(x => x.AttendanceSession.SessionDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<AttendanceRecord?> GetBySessionAndStudentAsync(int attendanceSessionId, int studentId, CancellationToken cancellationToken = default)
        {
            return await _context.AttendanceRecords
                .Include(x => x.Student)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(x =>
                    x.AttendanceSessionId == attendanceSessionId &&
                    x.StudentId == studentId,
                    cancellationToken);
        }

        public async Task AddAsync(AttendanceRecord entity, CancellationToken cancellationToken = default)
        {
            await _context.AttendanceRecords.AddAsync(entity, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<AttendanceRecord> entities, CancellationToken cancellationToken = default)
        {
            await _context.AttendanceRecords.AddRangeAsync(entities, cancellationToken);
        }

        public void Update(AttendanceRecord entity)
        {
            _context.AttendanceRecords.Update(entity);
        }

        public void Remove(AttendanceRecord entity)
        {
            _context.AttendanceRecords.Remove(entity);
        }

        public void RemoveRange(IEnumerable<AttendanceRecord> entities)
        {
            _context.AttendanceRecords.RemoveRange(entities);
        }
    }
}