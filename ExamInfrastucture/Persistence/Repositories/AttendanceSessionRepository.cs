using ExamApplication.Interfaces.Repository;
using ExamDomain.Entities;
using ExamInfrastucture.DAL;
using Microsoft.EntityFrameworkCore;

namespace ExamInfrastucture.Persistence.Repositories
{
    public class AttendanceSessionRepository : IAttendanceSessionRepository
    {
        private readonly AppDbContext _context;

        public AttendanceSessionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AttendanceSession?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.AttendanceSessions
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<AttendanceSession?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.AttendanceSessions
                .Include(x => x.ClassRoom)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                .Include(x => x.Records)
                    .ThenInclude(r => r.Student)
                        .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<AttendanceSession>> GetByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default)
        {
            return await _context.AttendanceSessions
                .Include(x => x.ClassRoom)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                .Include(x => x.Records)
                .Where(x => x.ClassRoomId == classRoomId)
                .OrderByDescending(x => x.SessionDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<AttendanceSession>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
        {
            return await _context.AttendanceSessions
                .Include(x => x.ClassRoom)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                .Include(x => x.Records.Where(r => r.StudentId == studentId))
                    .ThenInclude(r => r.Student)
                        .ThenInclude(s => s.User)
                .Where(x => x.Records.Any(r => r.StudentId == studentId))
                .OrderByDescending(x => x.SessionDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<AttendanceSession>> GetByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default)
        {
            return await _context.AttendanceSessions
                .Include(x => x.ClassRoom)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                .Include(x => x.Records)
                .Where(x => x.TeacherId == teacherId)
                .OrderByDescending(x => x.SessionDate)
                .ToListAsync(cancellationToken);
        }
        // YENI
        public async Task<List<AttendanceSession>> GetBoardSessionsAsync(
            int classRoomId,
            int subjectId,
            int teacherId,
            int year,
            int month,
            CancellationToken cancellationToken = default)
        {
            return await _context.AttendanceSessions
                .Include(x => x.ClassRoom)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                .Include(x => x.Records)
                    .ThenInclude(r => r.Student)
                        .ThenInclude(s => s.User)
                .Where(x =>
                    x.ClassRoomId == classRoomId &&
                    x.SubjectId == subjectId &&
                    x.TeacherId == teacherId &&
                    x.SessionDate.Year == year &&
                    x.SessionDate.Month == month)
                .OrderBy(x => x.SessionDate)
                .ThenBy(x => x.StartTime)
                .ToListAsync(cancellationToken);
        }
        // YENI
        public async Task<AttendanceSession?> GetByClassRoomSubjectTeacherAndDateAsync(
            int classRoomId,
            int subjectId,
            int teacherId,
            DateTime sessionDate,
            CancellationToken cancellationToken = default)
        {
            return await _context.AttendanceSessions
                .Include(x => x.ClassRoom)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                .Include(x => x.Records)
                    .ThenInclude(r => r.Student)
                        .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(x =>
                    x.ClassRoomId == classRoomId &&
                    x.SubjectId == subjectId &&
                    x.TeacherId == teacherId &&
                    x.SessionDate.Date == sessionDate.Date,
                    cancellationToken);
        }

        // YENI
        public async Task<List<AttendanceSession>> GetByClassRoomSubjectTeacherAndMonthAsync(
            int classRoomId,
            int subjectId,
            int teacherId,
            int year,
            int month,
            CancellationToken cancellationToken = default)
        {
            return await _context.AttendanceSessions
                .Include(x => x.ClassRoom)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                .Include(x => x.Records)
                    .ThenInclude(r => r.Student)
                        .ThenInclude(s => s.User)
                .Where(x =>
                    x.ClassRoomId == classRoomId &&
                    x.SubjectId == subjectId &&
                    x.TeacherId == teacherId &&
                    x.SessionDate.Year == year &&
                    x.SessionDate.Month == month)
                .OrderBy(x => x.SessionDate)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(AttendanceSession entity, CancellationToken cancellationToken = default)
        {
            await _context.AttendanceSessions.AddAsync(entity, cancellationToken);
        }

        public void Update(AttendanceSession entity)
        {
            _context.AttendanceSessions.Update(entity);
        }

        public void Remove(AttendanceSession entity)
        {
            _context.AttendanceSessions.Remove(entity);
        }
    }
}