using ExamApplication.Interfaces.Services;
using ExamDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
namespace ExamApplication.Helper
{
    /// <summary>
    /// ASP.NET Core Identity PasswordHasher istifadə edərək şifrələri hash edir və yoxlayır.
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        private readonly Microsoft.AspNetCore.Identity.PasswordHasher<User> _passwordHasher;

        public PasswordHasher()
        {
            _passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
        }

        /// <summary>
        /// Verilmiş plain text şifrəni hash edir.
        /// </summary>
        public string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("Şifrə boş ola bilməz.");

            return _passwordHasher.HashPassword(null!, password);
        }

        /// <summary>
        /// Verilmiş plain text şifrə ilə hash olunmuş şifrəni müqayisə edir.
        /// </summary>
        public bool Verify(string password, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (string.IsNullOrWhiteSpace(passwordHash))
                return false;

            var result = _passwordHasher.VerifyHashedPassword(null!, passwordHash, password);

            return result == PasswordVerificationResult.Success
                || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
