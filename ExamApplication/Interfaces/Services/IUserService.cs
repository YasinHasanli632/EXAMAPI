using ExamApplication.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    /// <summary>
    /// Sistem daxilində user account-larının idarə olunmasına cavabdeh olan servis kontraktıdır.
    /// Admin, teacher və student rollarına aid əsas hesab əməliyyatları burada idarə olunur.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Yeni user yaradır.
        /// Bu metod əsas account məlumatlarını yaradır və rolu təyin edir.
        /// Teacher və Student profil hissəsi ayrıca servis tərəfindən tamamlana bilər.
        /// </summary>
        /// <param name="request">Yeni user yaratmaq üçün göndərilən məlumatlar</param>
        /// <param name="cancellationToken">Async əməliyyatı dayandırmaq üçün token</param>
        /// <returns>Yaradılmış user-in detail məlumatları</returns>
        Task<UserDetailDto> CreateAsync(
            CreateUserRequestDto request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sistemdə olan bütün user-ləri qaytarır.
        /// Admin paneldə ümumi user siyahısını göstərmək üçün istifadə olunur.
        /// </summary>
        /// <param name="cancellationToken">Async əməliyyatı dayandırmaq üçün token</param>
        /// <returns>User siyahısı</returns>
        Task<List<UserListItemDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Verilmiş role-a uyğun olan user-ləri qaytarır.
        /// Məsələn yalnız Admin, yalnız Teacher və ya yalnız Student user-ləri filtr etmək üçün istifadə olunur.
        /// </summary>
        /// <param name="role">Filter üçün role adı</param>
        /// <param name="cancellationToken">Async əməliyyatı dayandırmaq üçün token</param>
        /// <returns>Verilmiş role-a uyğun user siyahısı</returns>
        Task<List<UserListItemDto>> GetByRoleAsync(
            string role,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Verilmiş Id-yə görə user-in detail məlumatını qaytarır.
        /// </summary>
        /// <param name="userId">Axtarılan user-in Id-si</param>
        /// <param name="cancellationToken">Async əməliyyatı dayandırmaq üçün token</param>
        /// <returns>User detail məlumatı</returns>
        Task<UserDetailDto> GetByIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Mövcud user-in əsas hesab məlumatlarını yeniləyir.
        /// Username, email, role və super admin statusu burada dəyişdirilə bilər.
        /// </summary>
        /// <param name="request">Yenilənəcək user məlumatları</param>
        /// <param name="cancellationToken">Async əməliyyatı dayandırmaq üçün token</param>
        /// <returns>Yenilənmiş user detail məlumatı</returns>
        Task<UserDetailDto> UpdateAsync(
            UpdateUserRequestDto request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// User-in aktiv/passiv statusunu dəyişir.
        /// Bu metod soft delete və ya user bloklama məqsədi ilə istifadə oluna bilər.
        /// </summary>
        /// <param name="request">Status dəyişmə məlumatları</param>
        /// <param name="cancellationToken">Async əməliyyatı dayandırmaq üçün token</param>
        Task ChangeStatusAsync(
            ChangeUserStatusRequestDto request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Verilmiş Id-yə görə user-i sistemdən silir.
        /// Real sistemdə bu əməliyyat çox vaxt soft delete və ya deactivate kimi implement olunur.
        /// </summary>
        // <param name="userId">Silinəcək user-in Id-si</param>
        // <param name="cancellationToken">Async əməliyyatı dayandırmaq üçün token</param>
        Task DeleteAsync(
            int userId,
            CancellationToken cancellationToken = default);
    }
}

