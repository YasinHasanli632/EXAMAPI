using ExamApplication.DTO.Auth;
using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    /// <summary>
    /// Sistem daxilində authentication (login və token əməliyyatları)
    /// ilə bağlı business logic-i idarə edən servis kontraktıdır.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// İstifadəçinin login məlumatlarını yoxlayır.
        /// 
        /// Username və ya email + password yoxlanılır.
        /// Əgər məlumatlar düzgündürsə JWT token yaradılır
        /// və login cavabı qaytarılır.
        /// </summary>
        // <param name="request">Login üçün göndərilən məlumatlar</param>
        // <param name="cancellationToken">Async əməliyyatı dayandırmaq üçün token</param>
        /// <returns>JWT token və istifadəçi məlumatları</returns>
        Task<LoginResponseDto> LoginAsync(
            LoginRequestDto request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Hal-hazırda login olmuş istifadəçinin məlumatlarını qaytarır.
        /// 
        /// Adətən JWT token içindən user id götürülür və
        /// həmin user database-dən tapılır.
        /// </summary>
        // <param name="cancellationToken">Async əməliyyatı dayandırmaq üçün token</param>
        /// <returns>Login olmuş istifadəçinin məlumatları</returns>
        Task<CurrentUserDto> GetCurrentUserAsync(
            CancellationToken cancellationToken = default);

       
        /// Verilmiş istifadəçi üçün JWT access token yaradır.
        /// 
        /// Bu metod əsasən Login əməliyyatı zamanı istifadə olunur.
        /// Token içində user id və role kimi məlumatlar yerləşdirilir.
      
        // <param name="user">Token yaradılacaq istifadəçi</param>
        // <param name="cancellationToken">Async əməliyyatı dayandırmaq üçün token</param>
        // <returns>JWT token string</returns>
        Task<string> GenerateJwtTokenAsync(
            User user,
            CancellationToken cancellationToken = default);
    }
}
