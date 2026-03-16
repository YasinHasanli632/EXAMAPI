using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Repository
{
    public interface ITeacherClassRoomRepository
    {
        // Id-yə görə teacher-class əlaqəsini gətirir
        Task<TeacherClassRoom?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // Teacher və class cütünə görə əlaqəni gətirir
        Task<TeacherClassRoom?> GetByTeacherAndClassRoomAsync(int teacherId, int classRoomId, CancellationToken cancellationToken = default);

        // Müəllimin bütün class əlaqələrini gətirir
        Task<List<TeacherClassRoom>> GetByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default);

        // Sinifə bağlı bütün teacher əlaqələrini gətirir
        Task<List<TeacherClassRoom>> GetByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default);

        // Teacher-class əlaqəsinin mövcud olub-olmadığını yoxlayır
        Task<bool> ExistsAsync(int teacherId, int classRoomId, CancellationToken cancellationToken = default);

        // Yeni teacher-class əlaqəsi əlavə edir
        Task AddAsync(TeacherClassRoom teacherClassRoom, CancellationToken cancellationToken = default);

        // Teacher-class əlaqəsini update üçün context-ə işarələyir
        void Update(TeacherClassRoom teacherClassRoom);

        // Teacher-class əlaqəsini silmək üçün context-ə işarələyir
        void Remove(TeacherClassRoom teacherClassRoom);
    }
}
