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
    public class TeacherClassRoomRepository : ITeacherClassRoomRepository
    {
        private readonly AppDbContext _context;

        public TeacherClassRoomRepository(AppDbContext context)
        {
            _context = context;
        }

        // Id-yə görə teacher-class əlaqəsini gətirir
        public async Task<TeacherClassRoom?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.TeacherClassRooms
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Include(x => x.ClassRoom)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // Teacher və class cütünə görə əlaqəni gətirir
        public async Task<TeacherClassRoom?> GetByTeacherAndClassRoomAsync(int teacherId, int classRoomId, CancellationToken cancellationToken = default)
        {
            return await _context.TeacherClassRooms
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Include(x => x.ClassRoom)
                .FirstOrDefaultAsync(x => x.TeacherId == teacherId && x.ClassRoomId == classRoomId, cancellationToken);
        }

        // Müəllimin bütün class əlaqələrini gətirir
        public async Task<List<TeacherClassRoom>> GetByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default)
        {
            return await _context.TeacherClassRooms
                .Include(x => x.ClassRoom)
                .Where(x => x.TeacherId == teacherId)
                .OrderBy(x => x.ClassRoom.Grade)
                .ThenBy(x => x.ClassRoom.Name)
                .ToListAsync(cancellationToken);
        }

        // Sinifə bağlı bütün teacher əlaqələrini gətirir
        public async Task<List<TeacherClassRoom>> GetByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default)
        {
            return await _context.TeacherClassRooms
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .Where(x => x.ClassRoomId == classRoomId)
                .OrderBy(x => x.Teacher.FullName)
                .ToListAsync(cancellationToken);
        }

        // Teacher-class əlaqəsinin mövcud olub-olmadığını yoxlayır
        public async Task<bool> ExistsAsync(int teacherId, int classRoomId, CancellationToken cancellationToken = default)
        {
            return await _context.TeacherClassRooms
                .AnyAsync(x => x.TeacherId == teacherId && x.ClassRoomId == classRoomId, cancellationToken);
        }

        // Yeni teacher-class əlaqəsi əlavə edir
        public async Task AddAsync(TeacherClassRoom teacherClassRoom, CancellationToken cancellationToken = default)
        {
            await _context.TeacherClassRooms.AddAsync(teacherClassRoom, cancellationToken);
        }

        // Teacher-class əlaqəsini update üçün context-ə işarələyir
        public void Update(TeacherClassRoom teacherClassRoom)
        {
            _context.TeacherClassRooms.Update(teacherClassRoom);
        }

        // Teacher-class əlaqəsini silmək üçün context-ə işarələyir
        public void Remove(TeacherClassRoom teacherClassRoom)
        {
            _context.TeacherClassRooms.Remove(teacherClassRoom);
        }
    }
}
