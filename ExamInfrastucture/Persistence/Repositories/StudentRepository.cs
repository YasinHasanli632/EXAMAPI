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
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        // Id-yə görə student gətirir
        public async Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // UserId-yə görə student gətirir
        public async Task<Student?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        }

        // Id-yə görə student-i detail-lərlə gətirir
        public async Task<Student?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .Include(x => x.User)
                .Include(x => x.StudentClasses)
                    .ThenInclude(x => x.ClassRoom)
                .Include(x => x.StudentExams)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // Bütün student-ləri gətirir
        public async Task<List<Student>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .Include(x => x.User)
                .OrderBy(x => x.FullName)
                .ToListAsync(cancellationToken);
        }

        // Verilən sinifdəki student-ləri gətirir
        public async Task<List<Student>> GetStudentsByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default)
        {
            return await _context.StudentClasses
                .Where(x => x.ClassRoomId == classRoomId)
                .Select(x => x.Student)
                .Include(x => x.User)
                .OrderBy(x => x.FullName)
                .ToListAsync(cancellationToken);
        }

        // Student nömrəsinin mövcud olub-olmadığını yoxlayır
        public async Task<bool> ExistsByStudentNumberAsync(string studentNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .AnyAsync(x => x.StudentNumber == studentNumber, cancellationToken);
        }

        // Yeni student əlavə edir
        public async Task AddAsync(Student student, CancellationToken cancellationToken = default)
        {
            await _context.Students.AddAsync(student, cancellationToken);
        }

        // Student-i update üçün context-ə işarələyir
        public void Update(Student student)
        {
            _context.Students.Update(student);
        }

        // Student-i silmək üçün context-ə işarələyir
        public void Remove(Student student)
        {
            _context.Students.Remove(student);
        }
    }
}
