using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface ITeacherSubjectRepository
    {
        Task<TeacherSubject?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<TeacherSubject?> GetByTeacherAndSubjectAsync(int teacherId, int subjectId, CancellationToken cancellationToken = default);

        Task<List<TeacherSubject>> GetByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default);

        Task<List<TeacherSubject>> GetBySubjectIdAsync(int subjectId, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(int teacherId, int subjectId, CancellationToken cancellationToken = default);

        Task AddAsync(TeacherSubject teacherSubject, CancellationToken cancellationToken = default);

        void Update(TeacherSubject teacherSubject);

        void Remove(TeacherSubject teacherSubject);

        // YENI
        Task<List<TeacherSubject>> GetByTeacherIdsAsync(List<int> teacherIds, CancellationToken cancellationToken = default);

        // YENI
        Task<List<TeacherSubject>> GetByTeacherIdWithTeacherAsync(int teacherId, CancellationToken cancellationToken = default);

        // YENI
        Task RemoveRangeAsync(List<TeacherSubject> teacherSubjects, CancellationToken cancellationToken = default);
    }
}
