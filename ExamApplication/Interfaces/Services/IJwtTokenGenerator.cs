using ExamDomain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    /// <summary>
    /// JWT token yaratmaq üçün istifadə olunan kontraktdır.
    /// </summary>
    public interface IJwtTokenGenerator
    {
        /// <summary>
        /// Verilmiş istifadəçi məlumatlarına əsasən JWT token yaradır.
        /// </summary>
        // <param name="userInfo">Token içində saxlanılacaq istifadəçi məlumatları</param>
        /// <returns>JWT token string</returns>
        string GenerateToken(JwtUserInfo userInfo);
    }
}
