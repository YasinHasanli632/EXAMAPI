using ExamApplication.DTO.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    public interface IClassRoomService
    {
        // Yeni sinif yaradır
        Task<ClassRoomDto> CreateAsync(CreateClassRoomDto request, CancellationToken cancellationToken = default);

        // Mövcud sinifi yeniləyir
        Task<ClassRoomDto> UpdateAsync(UpdateClassRoomDto request, CancellationToken cancellationToken = default);

        // Id-yə görə sinifi gətirir
        Task<ClassRoomDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // Id-yə görə sinifi bütün detalları ilə birlikdə gətirir
        Task<ClassRoomDetailsDto> GetDetailsByIdAsync(int id, CancellationToken cancellationToken = default);

        // Bütün sinifləri qaytarır
        Task<List<ClassRoomDto>> GetAllAsync(CancellationToken cancellationToken = default);

        // Tələbəyə aid bütün sinifləri qaytarır
        Task<List<ClassRoomDto>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);

        // Müəllimə aid bütün sinifləri qaytarır
        Task<List<ClassRoomDto>> GetByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default);

        // Sinfə tələbə təyin edir
        Task AssignStudentAsync(AssignStudentToClassRoomDto request, CancellationToken cancellationToken = default);

        // Sinifdən tələbəni çıxarır
        Task RemoveStudentAsync(RemoveStudentFromClassRoomDto request, CancellationToken cancellationToken = default);

        // Sinfə müəllim təyin edir
        Task AssignTeacherAsync(AssignTeacherToClassRoomDto request, CancellationToken cancellationToken = default);

        // Sinifdən müəllimi çıxarır
        Task RemoveTeacherAsync(RemoveTeacherFromClassRoomDto request, CancellationToken cancellationToken = default);

        // Sinifdə olan bütün tələbələri qaytarır
        Task<List<ClassRoomStudentDto>> GetStudentsByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default);

        // Sinifə bağlı bütün müəllimləri qaytarır
        Task<List<ClassRoomTeacherDto>> GetTeachersByClassRoomIdAsync(int classRoomId, CancellationToken cancellationToken = default);

        // Sinifi silir
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
