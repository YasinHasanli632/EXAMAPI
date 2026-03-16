using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    /// <summary>
    /// Şifrələri hash etmək və hash-lənmiş şifrəni yoxlamaq üçün istifadə olunan kontraktdır.
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Verilmiş plain text şifrəni hash edir.
        /// </summary>
        // <param name="password">İstifadəçinin plain text şifrəsi</param>
        /// <returns>Hash olunmuş şifrə</returns>
        string Hash(string password);

        /// <summary>
        /// Verilmiş plain text şifrə ilə hash olunmuş şifrəni müqayisə edir.
        /// </summary>
        // <param name="password">İstifadəçinin daxil etdiyi plain text şifrə</param>
        // <param name="passwordHash">Database-də saxlanılan hash olunmuş şifrə</param>
        /// <returns>Uyğundursa true, deyilsə false</returns>
        bool Verify(string password, string passwordHash);
    }
}
