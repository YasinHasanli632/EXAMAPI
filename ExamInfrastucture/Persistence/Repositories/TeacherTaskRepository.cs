using ExamApplication.Interfaces.Repository;
using ExamDomain.Entities;
using ExamDomain.Enum;
using ExamInfrastucture.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamInfrastucture.Persistence.Repositories
{
    public class TeacherTaskRepository : ITeacherTaskRepository
    {
        private readonly AppDbContext _context;

        public TeacherTaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TeacherTask?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.TeacherTasks
                .Include(x => x.Teacher)
                    .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<TeacherTask>> GetByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default)
        {
            return await _context.TeacherTasks
                .Where(x => x.TeacherId == teacherId)
                .OrderBy(x => x.IsCompleted)
                .ThenBy(x => x.DueDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<TeacherTask>> GetPendingByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default)
        {
            return await _context.TeacherTasks
                .Where(x => x.TeacherId == teacherId
                         && x.Status != TeacherTaskStatus.Completed
                         && !x.IsCompleted)
                .OrderBy(x => x.DueDate)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(TeacherTask teacherTask, CancellationToken cancellationToken = default)
        {
            await _context.TeacherTasks.AddAsync(teacherTask, cancellationToken);
        }

        public void Update(TeacherTask teacherTask)
        {
            _context.TeacherTasks.Update(teacherTask);
        }

        public void Remove(TeacherTask teacherTask)
        {
            _context.TeacherTasks.Remove(teacherTask);
        }
    }
}
