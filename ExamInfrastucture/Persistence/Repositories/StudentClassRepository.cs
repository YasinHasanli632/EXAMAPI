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

        // Id-yə görə student-class əlaqəsini gətirir
        public async Task<StudentClass?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.StudentClasses
                .Include(x => x.Student)
                .Include(x => x.ClassRoom)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // Student və class cütünə görə əlaqəni gətirir
        public async Task<StudentClass?> GetByStudentAndClassAsync(int studentId, int classRoomId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentClasses
                .Include(x => x.Student)
                .Include(x => x.ClassRoom)
                .FirstOrDefaultAsync(x => x.StudentId == studentId && x.ClassRoomId == classRoomId, cancellationToken);
        }

        // Student-in bütün class əlaqələrini gətirir
        public async Task<List<StudentClass>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentClasses
                .Include(x => x.ClassRoom)
                .Where(x => x.StudentId == studentId)
                .OrderBy(x => x.ClassRoom.Grade)
                .ThenBy(x => x.ClassRoom.Name)
                .ToListAsync(cancellationToken);
        }

        // Sinifdəki bütün student-class əlaqələrini gətirir
        public async Task<List<StudentClass>> GetByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentClasses
                .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                .Where(x => x.ClassRoomId == classRoomId)
                .OrderBy(x => x.Student.FullName)
                .ToListAsync(cancellationToken);
        }

        // Student-class əlaqəsinin mövcud olub-olmadığını yoxlayır
        public async Task<bool> ExistsAsync(int studentId, int classRoomId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentClasses
                .AnyAsync(x => x.StudentId == studentId && x.ClassRoomId == classRoomId, cancellationToken);
        }

        // Yeni student-class əlaqəsi əlavə edir
        public async Task AddAsync(StudentClass studentClass, CancellationToken cancellationToken = default)
        {
            await _context.StudentClasses.AddAsync(studentClass, cancellationToken);
        }

        // Student-class əlaqəsini update üçün context-ə işarələyir
        public void Update(StudentClass studentClass)
        {
            _context.StudentClasses.Update(studentClass);
        }

        // Student-class əlaqəsini silmək üçün context-ə işarələyir
        public void Remove(StudentClass studentClass)
        {
            _context.StudentClasses.Remove(studentClass);
        }
    }
}
