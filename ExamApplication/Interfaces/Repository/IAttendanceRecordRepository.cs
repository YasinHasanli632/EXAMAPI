using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface IAttendanceRecordRepository
    {
        Task<AttendanceRecord?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<AttendanceRecord>> GetBySessionIdAsync(int attendanceSessionId, CancellationToken cancellationToken = default);
        Task<List<AttendanceRecord>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
        Task<AttendanceRecord?> GetBySessionAndStudentAsync(int attendanceSessionId, int studentId, CancellationToken cancellationToken = default);

        Task AddAsync(AttendanceRecord entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<AttendanceRecord> entities, CancellationToken cancellationToken = default);

        void Update(AttendanceRecord entity);
        void Remove(AttendanceRecord entity);
        void RemoveRange(IEnumerable<AttendanceRecord> entities);
    }
}
