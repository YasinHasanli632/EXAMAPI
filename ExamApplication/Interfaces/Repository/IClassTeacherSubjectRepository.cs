using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface IClassTeacherSubjectRepository
    {
        // YENI
        Task<ClassTeacherSubject?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // YENI
        Task<List<ClassTeacherSubject>> GetByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default);

        // YENI
        Task<List<ClassTeacherSubject>> GetByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default);

        // YENI
        Task<List<ClassTeacherSubject>> GetBySubjectIdAsync(int subjectId, CancellationToken cancellationToken = default);

        // YENI
        Task<bool> ExistsAsync(int classRoomId, int teacherId, int subjectId, CancellationToken cancellationToken = default);

        // YENI
        Task AddAsync(ClassTeacherSubject entity, CancellationToken cancellationToken = default);

        // YENI
        Task AddRangeAsync(IEnumerable<ClassTeacherSubject> entities, CancellationToken cancellationToken = default);

        // YENI
        void Remove(ClassTeacherSubject entity);

        // YENI
        void RemoveRange(IEnumerable<ClassTeacherSubject> entities);
    }
}
