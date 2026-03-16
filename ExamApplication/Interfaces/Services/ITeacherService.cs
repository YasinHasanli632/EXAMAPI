using ExamApplication.DTO.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    public interface ITeacherService
    {
        // Yeni teacher yaradır
        Task<TeacherDto> CreateAsync(CreateTeacherDto request, CancellationToken cancellationToken = default);

        // Mövcud teacher-i yeniləyir
        Task<TeacherDto> UpdateAsync(UpdateTeacherDto request, CancellationToken cancellationToken = default);

        // Id-yə görə teacher gətirir
        Task<TeacherDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        // UserId-yə görə teacher gətirir
        Task<TeacherDto> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

        // Id-yə görə teacher detail gətirir
        Task<TeacherDetailsDto> GetDetailsByIdAsync(int id, CancellationToken cancellationToken = default);

        // Bütün teacher-ləri gətirir
        Task<List<TeacherDto>> GetAllAsync(CancellationToken cancellationToken = default);

        // Teacher-ə subject bağlayır
        Task AssignSubjectAsync(AssignSubjectToTeacherDto request, CancellationToken cancellationToken = default);

        // Teacher-dən subject çıxarır
        Task RemoveSubjectAsync(RemoveSubjectFromTeacherDto request, CancellationToken cancellationToken = default);

        // Teacher-ə class bağlayır
        Task AssignClassRoomAsync(AssignClassRoomToTeacherDto request, CancellationToken cancellationToken = default);

        // Teacher-dən class çıxarır
        Task RemoveClassRoomAsync(RemoveClassRoomFromTeacherDto request, CancellationToken cancellationToken = default);

        // Teacher-in bütün subject-lərini gətirir
        Task<List<TeacherSubjectDto>> GetSubjectsByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default);

        // Teacher-in bütün class-larını gətirir
        Task<List<TeacherClassRoomDto>> GetClassRoomsByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default);

        // Teacher-i silir
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
