using ExamApplication.Interfaces.Repository;
using ExamDomain.Entities;
using ExamInfrastucture.DAL;
using Microsoft.EntityFrameworkCore;

namespace ExamInfrastucture.Persistence.Repositories
{
    public class ClassTeacherSubjectRepository : IClassTeacherSubjectRepository
    {
        private readonly AppDbContext _context;

        public ClassTeacherSubjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ClassTeacherSubject?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ClassTeacherSubjects
                .Include(x => x.ClassRoom)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Include(x => x.Subject)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<ClassTeacherSubject>> GetByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default)
        {
            return await _context.ClassTeacherSubjects
                .Where(x => x.ClassRoomId == classRoomId)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Include(x => x.Subject)
                .OrderBy(x => x.Subject.Name)
                .ThenBy(x => x.Teacher.FullName)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ClassTeacherSubject>> GetByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default)
        {
            return await _context.ClassTeacherSubjects
                .Where(x => x.TeacherId == teacherId)
                .Include(x => x.ClassRoom)
                .Include(x => x.Subject)
                .OrderBy(x => x.ClassRoom.Grade)
                .ThenBy(x => x.ClassRoom.Name)
                .ThenBy(x => x.Subject.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ClassTeacherSubject>> GetBySubjectIdAsync(int subjectId, CancellationToken cancellationToken = default)
        {
            return await _context.ClassTeacherSubjects
                .Where(x => x.SubjectId == subjectId)
                .Include(x => x.ClassRoom)
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .OrderBy(x => x.ClassRoom.Grade)
                .ThenBy(x => x.ClassRoom.Name)
                .ThenBy(x => x.Teacher.FullName)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(int classRoomId, int teacherId, int subjectId, CancellationToken cancellationToken = default)
        {
            return await _context.ClassTeacherSubjects
                .AnyAsync(
                    x => x.ClassRoomId == classRoomId
                      && x.TeacherId == teacherId
                      && x.SubjectId == subjectId,
                    cancellationToken
                );
        }

        public async Task AddAsync(ClassTeacherSubject entity, CancellationToken cancellationToken = default)
        {
            await _context.ClassTeacherSubjects.AddAsync(entity, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<ClassTeacherSubject> entities, CancellationToken cancellationToken = default)
        {
            await _context.ClassTeacherSubjects.AddRangeAsync(entities, cancellationToken);
        }

        public void Remove(ClassTeacherSubject entity)
        {
            _context.ClassTeacherSubjects.Remove(entity);
        }

        public void RemoveRange(IEnumerable<ClassTeacherSubject> entities)
        {
            _context.ClassTeacherSubjects.RemoveRange(entities);
        }
    }
}