using ExamApplication.DTO.Class;
using ExamApplication.DTO.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        // YENI
        // UserId ilə detail gətirmək frontend üçün rahat olur
        Task<TeacherDetailsDto> GetDetailsByUserIdAsync(int userId, CancellationToken cancellationToken = default);

        // YENI
        // Admin paneldə siyahı üçün detail-lə list
        Task<List<TeacherDetailsDto>> GetAllDetailsAsync(CancellationToken cancellationToken = default);

        // YENI
        // Status dəyişmək üçün
        Task ChangeStatusAsync(ChangeTeacherStatusDto request, CancellationToken cancellationToken = default);

        // YENI
        // Mövcud assign/remove methodları qalır, amma full sync üçün bu daha rahatdır
        Task SyncSubjectsAsync(SyncTeacherSubjectsDto request, CancellationToken cancellationToken = default);

        // YENI
        Task SyncClassRoomsAsync(SyncTeacherClassRoomsDto request, CancellationToken cancellationToken = default);

        // YENI
        Task<List<TeacherTaskDto>> GetTasksByTeacherIdAsync(int teacherId, CancellationToken cancellationToken = default);

        // YENI
        Task<TeacherOverviewStatsDto> GetOverviewStatsAsync(int teacherId, CancellationToken cancellationToken = default);
        // YENI
        Task<TeacherDetailsDto> GetMeAsync(CancellationToken cancellationToken = default);

        // YENI
        Task<TeacherDashboardDto> GetMyDashboardAsync(CancellationToken cancellationToken = default);

        // YENI
        Task<List<TeacherMyClassRoomListItemDto>> GetMyClassRoomsAsync(CancellationToken cancellationToken = default);

        // YENI
        Task<ClassDetailDto> GetMyClassRoomDetailsAsync(int classRoomId, CancellationToken cancellationToken = default);
    }
}
