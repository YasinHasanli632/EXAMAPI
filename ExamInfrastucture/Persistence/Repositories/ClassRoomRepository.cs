using ExamApplication.Interfaces.Repository;
using ExamDomain.Entities;
using ExamInfrastucture.DAL;
using Microsoft.EntityFrameworkCore;

namespace ExamInfrastucture.Persistence.Repositories
{
    public class ClassRoomRepository : IClassRoomRepository
    {
        private readonly AppDbContext _context;

        public ClassRoomRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ClassRoom?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ClassRooms
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<ClassRoom?> GetByIdWithStudentsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ClassRooms
                .Include(x => x.StudentClasses.Where(sc => sc.IsActive))
                    .ThenInclude(x => x.Student)
                        .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<ClassRoom?> GetByIdWithTeacherAssignmentsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ClassRooms
                .Include(x => x.ClassTeacherSubjects.Where(cts => cts.IsActive))
                    .ThenInclude(x => x.Teacher)
                        .ThenInclude(x => x.User)
                .Include(x => x.ClassTeacherSubjects.Where(cts => cts.IsActive))
                    .ThenInclude(x => x.Subject)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<ClassRoom?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ClassRooms
                .Include(x => x.StudentClasses.Where(sc => sc.IsActive))
                    .ThenInclude(x => x.Student)
                        .ThenInclude(x => x.User)
                .Include(x => x.ClassTeacherSubjects.Where(cts => cts.IsActive))
                    .ThenInclude(x => x.Teacher)
                        .ThenInclude(x => x.User)
                .Include(x => x.ClassTeacherSubjects.Where(cts => cts.IsActive))
                    .ThenInclude(x => x.Subject)
                .Include(x => x.Exams)
                    .ThenInclude(x => x.Subject)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<ClassRoom>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.ClassRooms
                .OrderBy(x => x.Grade)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ClassRoom>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.ClassRooms
                .Include(x => x.StudentClasses.Where(sc => sc.IsActive))
                .Include(x => x.ClassTeacherSubjects.Where(cts => cts.IsActive))
                    .ThenInclude(x => x.Teacher)
                        .ThenInclude(x => x.User)
                .Include(x => x.ClassTeacherSubjects.Where(cts => cts.IsActive))
                    .ThenInclude(x => x.Subject)
                .Include(x => x.Exams)
                .OrderBy(x => x.Grade)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ClassRoom>> GetByAcademicYearAsync(string academicYear, CancellationToken cancellationToken = default)
        {
            return await _context.ClassRooms
                .Where(x => x.AcademicYear == academicYear)
                .OrderBy(x => x.Grade)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ClassRoom>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await _context.ClassRooms
                .Where(x => x.IsActive)
                .OrderBy(x => x.Grade)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.ClassRooms
                .AnyAsync(x => x.Name == name, cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, int excludeId, CancellationToken cancellationToken = default)
        {
            return await _context.ClassRooms
                .AnyAsync(x => x.Name == name && x.Id != excludeId, cancellationToken);
        }

        public async Task AddAsync(ClassRoom classRoom, CancellationToken cancellationToken = default)
        {
            await _context.ClassRooms.AddAsync(classRoom, cancellationToken);
        }

        public void Update(ClassRoom classRoom)
        {
            _context.ClassRooms.Update(classRoom);
        }

        public void Remove(ClassRoom classRoom)
        {
            _context.ClassRooms.Remove(classRoom);
        }

        public async Task<List<ClassRoom>> GetByIdsAsync(List<int> classRoomIds, CancellationToken cancellationToken = default)
        {
            if (classRoomIds == null || classRoomIds.Count == 0)
                return new List<ClassRoom>();

            return await _context.ClassRooms
                .Where(x => classRoomIds.Contains(x.Id))
                .OrderBy(x => x.Grade)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }
    }
}