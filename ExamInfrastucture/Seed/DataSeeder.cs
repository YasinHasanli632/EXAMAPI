using ExamApplication.Interfaces.Services;
using ExamDomain.Entities;
using ExamDomain.Enum;
using ExamInfrastucture.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamInfrastucture.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(
            AppDbContext context,
            IPasswordHasher passwordHasher,
            IConfiguration configuration)
        {
            // Əgər artıq superadmin varsa, yenidən yaratma
            var adminExists = await context.Users
                .AnyAsync(x => x.Username == "superadmin");

            if (adminExists)
            {
                return;
            }

            var seedUsername = configuration["SeedAdmin:Username"] ?? "superadmin";
            var seedEmail = configuration["SeedAdmin:Email"] ?? "superadmin@exam.local";
            var seedPassword = configuration["SeedAdmin:Password"] ?? "SuperAdmin123!";

            var passwordHash = passwordHasher.Hash(seedPassword);

            var admin = new User
            {
                Username = seedUsername,
                Email = seedEmail,
                PasswordHash = passwordHash,
                Role = UserRole.IsSuperAdmin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
