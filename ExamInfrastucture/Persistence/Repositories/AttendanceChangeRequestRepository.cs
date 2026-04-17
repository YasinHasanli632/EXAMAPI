using ExamApplication.Interfaces.Repository;
using ExamDomain.Entities;
using ExamInfrastucture.DAL;
using Microsoft.EntityFrameworkCore;

namespace ExamInfrastucture.Persistence.Repositories
{
    public class AttendanceChangeRequestRepository : IAttendanceChangeRequestRepository
    {
        private readonly AppDbContext _context;

        public AttendanceChangeRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AttendanceChangeRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.AttendanceChangeRequests
                .Include(x => x.ClassRoom)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                .Include(x => x.Student)
                    .ThenInclude(s => s.User)
                .Include(x => x.RequestedByTeacher)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // YENI
        public async Task<AttendanceChangeRequest?> GetPendingByAttendanceInfoAsync(
            int classRoomId,
            int subjectId,
            int teacherId,
            int studentId,
            DateTime attendanceDate,
            CancellationToken cancellationToken = default)
        {
            return await _context.AttendanceChangeRequests
                .FirstOrDefaultAsync(x =>
                    x.ClassRoomId == classRoomId &&
                    x.SubjectId == subjectId &&
                    x.TeacherId == teacherId &&
                    x.StudentId == studentId &&
                    x.AttendanceDate.Date == attendanceDate.Date &&
                    x.RequestStatus == "Pending",
                    cancellationToken);
        }

        // YENI
        public async Task<List<AttendanceChangeRequest>> GetFilteredAsync(
            string? requestStatus,
            int? classRoomId,
            int? subjectId,
            int? teacherId,
            DateTime? attendanceDateFrom,
            DateTime? attendanceDateTo,
            CancellationToken cancellationToken = default)
        {
            var query = _context.AttendanceChangeRequests
                .Include(x => x.ClassRoom)
                .Include(x => x.Subject)
                .Include(x => x.Teacher)
                .Include(x => x.Student)
                    .ThenInclude(s => s.User)
                .Include(x => x.RequestedByTeacher)
                    .ThenInclude(t => t.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(requestStatus))
            {
                var normalizedStatus = requestStatus.Trim();
                query = query.Where(x => x.RequestStatus == normalizedStatus);
            }

            if (classRoomId.HasValue && classRoomId.Value > 0)
                query = query.Where(x => x.ClassRoomId == classRoomId.Value);

            if (subjectId.HasValue && subjectId.Value > 0)
                query = query.Where(x => x.SubjectId == subjectId.Value);

            if (teacherId.HasValue && teacherId.Value > 0)
                query = query.Where(x => x.TeacherId == teacherId.Value);

            if (attendanceDateFrom.HasValue)
                query = query.Where(x => x.AttendanceDate.Date >= attendanceDateFrom.Value.Date);

            if (attendanceDateTo.HasValue)
                query = query.Where(x => x.AttendanceDate.Date <= attendanceDateTo.Value.Date);

            return await query
                .OrderByDescending(x => x.RequestedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(AttendanceChangeRequest entity, CancellationToken cancellationToken = default)
        {
            await _context.AttendanceChangeRequests.AddAsync(entity, cancellationToken);
        }

        public void Update(AttendanceChangeRequest entity)
        {
            _context.AttendanceChangeRequests.Update(entity);
        }
    }
}