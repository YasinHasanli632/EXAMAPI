using ExamApplication.Interfaces.Repository;
using ExamDomain.Entities;
using ExamInfrastucture.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamInfrastucture.Persistence.Repositories
{
    public class StudentClassRepository : IStudentClassRepository
    {
        private readonly AppDbContext _context;

        public StudentClassRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StudentClass?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.StudentClasses
                .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                .Include(x => x.ClassRoom)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<StudentClass?> GetByStudentAndClassAsync(int studentId, int classRoomId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentClasses
                .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                .Include(x => x.ClassRoom)
                .FirstOrDefaultAsync(x => x.StudentId == studentId && x.ClassRoomId == classRoomId, cancellationToken);
        }

        public async Task<List<StudentClass>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentClasses
                .Include(x => x.ClassRoom)
                .Where(x => x.StudentId == studentId)
                .OrderByDescending(x => x.IsActive)
                .ThenBy(x => x.ClassRoom.Grade)
                .ThenBy(x => x.ClassRoom.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<StudentClass>> GetByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentClasses
                .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                .Where(x => x.ClassRoomId == classRoomId)
                .OrderByDescending(x => x.IsActive)
                .ThenBy(x => x.Student.FullName)
                .ToListAsync(cancellationToken);
        }
        // YENI
        public async Task<bool> AreAllStudentsActiveInClassAsync(
            int classRoomId,
            List<int> studentIds,
            CancellationToken cancellationToken = default)
        {
            if (studentIds == null || studentIds.Count == 0)
                return false;

            var distinctStudentIds = studentIds
                .Where(x => x > 0)
                .Distinct()
                .ToList();

            var matchedCount = await _context.StudentClasses
                .Where(x => x.ClassRoomId == classRoomId
                            && x.IsActive
                            && distinctStudentIds.Contains(x.StudentId))
                .Select(x => x.StudentId)
                .Distinct()
                .CountAsync(cancellationToken);

            return matchedCount == distinctStudentIds.Count;
        }
        public async Task<List<StudentClass>> GetActiveByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentClasses
                .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                .Where(x => x.ClassRoomId == classRoomId && x.IsActive)
                .OrderBy(x => x.Student.FullName)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<StudentClass>> GetActiveByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentClasses
                .Include(x => x.ClassRoom)
                .Where(x => x.StudentId == studentId && x.IsActive)
                .OrderBy(x => x.ClassRoom.Grade)
                .ThenBy(x => x.ClassRoom.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<StudentClass>> GetByStudentIdsAsync(List<int> studentIds, CancellationToken cancellationToken = default)
        {
            if (studentIds == null || studentIds.Count == 0)
                return new List<StudentClass>();

            return await _context.StudentClasses
                .Include(x => x.ClassRoom)
                .Where(x => studentIds.Contains(x.StudentId))
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(int studentId, int classRoomId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentClasses
                .AnyAsync(x => x.StudentId == studentId && x.ClassRoomId == classRoomId && x.IsActive, cancellationToken);
        }

        public async Task AddAsync(StudentClass studentClass, CancellationToken cancellationToken = default)
        {
            await _context.StudentClasses.AddAsync(studentClass, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<StudentClass> studentClasses, CancellationToken cancellationToken = default)
        {
            await _context.StudentClasses.AddRangeAsync(studentClasses, cancellationToken);
        }

        public void Update(StudentClass studentClass)
        {
            _context.StudentClasses.Update(studentClass);
        }

        public void Remove(StudentClass studentClass)
        {
            _context.StudentClasses.Remove(studentClass);
        }

        public void RemoveRange(IEnumerable<StudentClass> studentClasses)
        {
            _context.StudentClasses.RemoveRange(studentClasses);
        }
    }
}
