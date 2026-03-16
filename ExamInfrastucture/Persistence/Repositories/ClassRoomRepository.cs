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
    public class ClassRoomRepository : IClassRoomRepository
    {
        private readonly AppDbContext _context;

        public ClassRoomRepository(AppDbContext context)
        {
            _context = context;
        }

        // Id-yə görə sinif gətirir
        public async Task<ClassRoom?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ClassRooms
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // Id-yə görə sinifi tələbələri ilə birlikdə gətirir
        public async Task<ClassRoom?> GetByIdWithStudentsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.ClassRooms
                .Include(x => x.StudentClasses)
                    .ThenInclude(x => x.Student)
                        .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        // Bütün sinifləri gətirir
        public async Task<List<ClassRoom>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.ClassRooms
                .OrderBy(x => x.Grade)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        // Sinif adının mövcud olub-olmadığını yoxlayır
        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.ClassRooms
                .AnyAsync(x => x.Name == name, cancellationToken);
        }

        // Yeni sinif əlavə edir
        public async Task AddAsync(ClassRoom classRoom, CancellationToken cancellationToken = default)
        {
            await _context.ClassRooms.AddAsync(classRoom, cancellationToken);
        }

        // Sinifi update üçün context-ə işarələyir
        public void Update(ClassRoom classRoom)
        {
            _context.ClassRooms.Update(classRoom);
        }

        // Sinifi silmək üçün context-ə işarələyir
        public void Remove(ClassRoom classRoom)
        {
            _context.ClassRooms.Remove(classRoom);
        }
    }
}
