using ExamDomain.Common;
using ExamDomain.Entities;
using ExamDomain.Enum;
using Microsoft.EntityFrameworkCore;

namespace ExamInfrastucture.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // User hesabları cədvəli
        public DbSet<User> Users { get; set; }

        // Tələbə profilləri cədvəli
        public DbSet<Student> Students { get; set; }

        // Müəllim profilləri cədvəli
        public DbSet<Teacher> Teachers { get; set; }

        // Siniflər cədvəli
        public DbSet<ClassRoom> ClassRooms { get; set; }

        // Fənlər cədvəli
        public DbSet<Subject> Subjects { get; set; }

        // Müəllim-fənn əlaqə cədvəli
        public DbSet<TeacherSubject> TeacherSubjects { get; set; }

        // Tələbə-sinif əlaqə cədvəli
        public DbSet<StudentClass> StudentClasses { get; set; }

        // Müəllim-sinif əlaqə cədvəli
        public DbSet<TeacherClassRoom> TeacherClassRooms { get; set; }

        // İmtahanlar cədvəli
        public DbSet<Exam> Exams { get; set; }

        // İmtahan sualları cədvəli
        public DbSet<ExamQuestion> ExamQuestions { get; set; }

        // Sual variantları cədvəli
        public DbSet<ExamOption> ExamOptions { get; set; }

        // Tələbənin imtahana girişi/nəticəsi cədvəli
        public DbSet<StudentExam> StudentExams { get; set; }

        // Tələbənin sual cavabları cədvəli
        public DbSet<StudentAnswer> StudentAnswers { get; set; }

        // İmtahan giriş kodları cədvəli
        public DbSet<ExamAccessCode> ExamAccessCodes { get; set; }

        // Bildirişlər cədvəli
        public DbSet<Notification> Notifications { get; set; }

        // Audit log cədvəli
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureUser(modelBuilder);
            ConfigureStudent(modelBuilder);
            ConfigureTeacher(modelBuilder);
            ConfigureClassRoom(modelBuilder);
            ConfigureSubject(modelBuilder);
            ConfigureTeacherSubject(modelBuilder);
            ConfigureStudentClass(modelBuilder);
            ConfigureTeacherClassRoom(modelBuilder);
            ConfigureExam(modelBuilder);
            ConfigureExamQuestion(modelBuilder);
            ConfigureExamOption(modelBuilder);
            ConfigureStudentExam(modelBuilder);
            ConfigureStudentAnswer(modelBuilder);
            ConfigureExamAccessCode(modelBuilder);
            ConfigureNotification(modelBuilder);
            ConfigureAuditLog(modelBuilder);
        }

        private static void ConfigureUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Username)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Role)
                    .IsRequired();

                entity.Property(x => x.IsActive)
                    .HasDefaultValue(true);

                entity.HasIndex(x => x.Username)
                    .IsUnique();

                entity.HasIndex(x => x.Email)
                    .IsUnique();

                entity.HasOne(x => x.Student)
                    .WithOne(x => x.User)
                    .HasForeignKey<Student>(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Teacher)
                    .WithOne(x => x.User)
                    .HasForeignKey<Teacher>(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureStudent(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Students");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.FullName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.DateOfBirth)
                    .IsRequired();

                entity.Property(x => x.StudentNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(x => x.UserId)
                    .IsUnique();

                entity.HasIndex(x => x.StudentNumber)
                    .IsUnique();
            });
        }

        private static void ConfigureTeacher(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.ToTable("Teachers");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.FullName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Department)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasIndex(x => x.UserId)
                    .IsUnique();
            });
        }

        private static void ConfigureClassRoom(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClassRoom>(entity =>
            {
                entity.ToTable("ClassRooms");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.Grade)
                    .IsRequired();

                entity.HasIndex(x => x.Name)
                    .IsUnique();
            });
        }

        private static void ConfigureSubject(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("Subjects");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasIndex(x => x.Name)
                    .IsUnique();
            });
        }

        private static void ConfigureTeacherSubject(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeacherSubject>(entity =>
            {
                entity.ToTable("TeacherSubjects");

                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.Teacher)
                    .WithMany(x => x.TeacherSubjects)
                    .HasForeignKey(x => x.TeacherId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Subject)
                    .WithMany(x => x.TeacherSubjects)
                    .HasForeignKey(x => x.SubjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => new { x.TeacherId, x.SubjectId })
                    .IsUnique();
            });
        }

        private static void ConfigureStudentClass(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentClass>(entity =>
            {
                entity.ToTable("StudentClasses");

                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.Student)
                    .WithMany(x => x.StudentClasses)
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.ClassRoom)
                    .WithMany(x => x.StudentClasses)
                    .HasForeignKey(x => x.ClassRoomId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => new { x.StudentId, x.ClassRoomId })
                    .IsUnique();
            });
        }

        private static void ConfigureTeacherClassRoom(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeacherClassRoom>(entity =>
            {
                entity.ToTable("TeacherClassRooms");

                entity.HasKey(x => x.Id);

                entity.HasIndex(x => new { x.TeacherId, x.ClassRoomId })
                    .IsUnique();

                entity.HasOne(x => x.Teacher)
                    .WithMany(x => x.TeacherClassRooms)
                    .HasForeignKey(x => x.TeacherId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.ClassRoom)
                    .WithMany(x => x.TeacherClassRooms)
                    .HasForeignKey(x => x.ClassRoomId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureExam(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Exam>(entity =>
            {
                entity.ToTable("Exams");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(x => x.StartTime)
                    .IsRequired();

                entity.Property(x => x.EndTime)
                    .IsRequired();

                entity.Property(x => x.DurationMinutes)
                    .IsRequired();

                entity.Property(x => x.Status)
                    .IsRequired();

                entity.HasOne(x => x.Subject)
                    .WithMany(x => x.Exams)
                    .HasForeignKey(x => x.SubjectId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Teacher)
                    .WithMany(x => x.CreatedExams)
                    .HasForeignKey(x => x.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigureExamQuestion(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExamQuestion>(entity =>
            {
                entity.ToTable("ExamQuestions");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.QuestionText)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(x => x.QuestionType)
                    .IsRequired();

                entity.Property(x => x.Points)
                    .IsRequired();

                entity.HasOne(x => x.Exam)
                    .WithMany(x => x.Questions)
                    .HasForeignKey(x => x.ExamId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureExamOption(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExamOption>(entity =>
            {
                entity.ToTable("ExamOptions");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.OptionText)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(x => x.IsCorrect)
                    .IsRequired();

                entity.HasOne(x => x.ExamQuestion)
                    .WithMany(x => x.Options)
                    .HasForeignKey(x => x.ExamQuestionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureStudentExam(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentExam>(entity =>
            {
                entity.ToTable("StudentExams");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.StartTime)
                    .IsRequired();

                entity.Property(x => x.Score)
                    .HasColumnType("decimal(18,2)");

                entity.Property(x => x.IsCompleted)
                    .HasDefaultValue(false);

                entity.HasOne(x => x.Student)
                    .WithMany(x => x.StudentExams)
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Exam)
                    .WithMany(x => x.StudentExams)
                    .HasForeignKey(x => x.ExamId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.StudentId, x.ExamId })
                    .IsUnique();
            });
        }

        private static void ConfigureStudentAnswer(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentAnswer>(entity =>
            {
                entity.ToTable("StudentAnswers");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.AnswerText)
                    .HasMaxLength(4000);

                entity.Property(x => x.PointsAwarded)
                    .HasColumnType("decimal(18,2)");

                entity.HasOne(x => x.StudentExam)
                    .WithMany(x => x.Answers)
                    .HasForeignKey(x => x.StudentExamId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.ExamQuestion)
                    .WithMany(x => x.StudentAnswers)
                    .HasForeignKey(x => x.ExamQuestionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.SelectedOption)
                    .WithMany()
                    .HasForeignKey(x => x.SelectedOptionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.StudentExamId, x.ExamQuestionId })
                    .IsUnique();
            });
        }

        private static void ConfigureExamAccessCode(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExamAccessCode>(entity =>
            {
                entity.ToTable("ExamAccessCodes");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.AccessCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.IsUsed)
                    .HasDefaultValue(false);

                entity.Property(x => x.ExpireAt)
                    .IsRequired();

                entity.HasOne(x => x.Exam)
     .WithMany()
     .HasForeignKey(x => x.ExamId)
     .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Student)
                    .WithMany()
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => x.AccessCode);

                entity.HasIndex(x => new { x.ExamId, x.StudentId })
                    .IsUnique();
            });
        }

        private static void ConfigureNotification(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notifications");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Message)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(x => x.Type)
                    .IsRequired();

                entity.Property(x => x.IsRead)
                    .HasDefaultValue(false);

                entity.HasOne(x => x.User)
                    .WithMany(x => x.Notifications)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureAuditLog(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLogs");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Action)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.EntityName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.EntityId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.ActionTime)
                    .IsRequired();

                entity.Property(x => x.Changes)
                    .HasColumnType("nvarchar(max)");

                entity.HasOne(x => x.User)
    .WithMany()
    .HasForeignKey(x => x.UserId)
    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            ApplyAuditInfo();
            return base.SaveChanges();
        }

        private void ApplyAuditInfo()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(x => x.Entity is AuditableEntity &&
                            (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (AuditableEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}