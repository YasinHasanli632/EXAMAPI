using ExamDomain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    /// <summary>
    /// Hal-hazırda request göndərən istifadəçinin token məlumatlarını qaytarır.
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// Cari request-in token məlumatlarından istifadəçi məlumatını çıxarır.
        /// </summary>
        /// <returns>Token içindən çıxarılan istifadəçi məlumatları</returns>
        JwtUserInfo? GetCurrentUser();
    }
}
