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

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<ClassRoom> ClassRooms { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<TeacherSubject> TeacherSubjects { get; set; }
        public DbSet<StudentClass> StudentClasses { get; set; }
        public DbSet<ClassTeacherSubject> ClassTeacherSubjects { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<ExamOption> ExamOptions { get; set; }
        public DbSet<StudentExam> StudentExams { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        public DbSet<StudentAnswerOption> StudentAnswerOptions { get; set; }
        public DbSet<ExamAccessCode> ExamAccessCodes { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<AttendanceChangeRequest> AttendanceChangeRequests { get; set; }
        public DbSet<TeacherTask> TeacherTasks { get; set; }
        public DbSet<AttendanceSession> AttendanceSessions { get; set; }
        public DbSet<StudentTask> StudentTasks { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        // YENI
        // YENI
        // YENI
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<ExamSecurityLog> ExamSecurityLogs { get; set; }
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
            ConfigureClassTeacherSubject(modelBuilder);
            ConfigureExam(modelBuilder);
            ConfigureExamQuestion(modelBuilder);
            ConfigureExamOption(modelBuilder);
            ConfigureStudentExam(modelBuilder);
            ConfigureStudentAnswer(modelBuilder);
            ConfigureStudentAnswerOption(modelBuilder);
            ConfigureExamAccessCode(modelBuilder);
            ConfigureNotification(modelBuilder);
            ConfigureAuditLog(modelBuilder);
            ConfigureTeacherTask(modelBuilder);
            ConfigureAttendanceSession(modelBuilder); // YENI
            ConfigureAttendanceChangeRequest(modelBuilder); // YENI
            ConfigureAttendanceRecord(modelBuilder); // YENI
            ConfigureStudentTask(modelBuilder);
            // YENI
            ConfigureExamSecurityLog(modelBuilder);
            // YENI
            ConfigureSystemSetting(modelBuilder);
            // YENI
            ConfigureRefreshToken(modelBuilder);
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

                entity.Property(x => x.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.BirthDate)
                    .IsRequired(false);

                entity.Property(x => x.PhoneNumber)
                    .HasMaxLength(30)
                    .IsRequired(false);

                entity.Property(x => x.PhotoUrl)
                    .HasMaxLength(500)
                    .IsRequired(false);

                entity.Property(x => x.Details)
                    .HasMaxLength(2000)
                    .IsRequired(false);

                entity.Property(x => x.FatherName)
                    .HasMaxLength(100)
                    .IsRequired(false);

                entity.Property(x => x.Country)
                    .HasMaxLength(100)
                    .IsRequired(false);

                entity.HasIndex(x => x.Username)
                    .IsUnique();

                entity.HasIndex(x => x.Email)
                    .IsUnique();
                // YENI
                entity.HasMany(x => x.RefreshTokens)
                    .WithOne(x => x.User)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
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

                entity.Property(x => x.Status)
                    .IsRequired()
                    .HasConversion<int>()
                    .HasDefaultValue(StudentStatus.Active);

                entity.Property(x => x.Notes)
                    .HasMaxLength(2000)
                    .IsRequired(false);

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

                entity.Property(x => x.Status)
                    .IsRequired()
                    .HasConversion<int>()
                    .HasDefaultValue(TeacherStatus.Active);

                entity.Property(x => x.Specialization)
                    .HasMaxLength(150)
                    .IsRequired(false);

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

                entity.Property(x => x.IsActive)
                    .HasDefaultValue(true);

                entity.Property(x => x.Room)
                    .HasMaxLength(100)
                    .IsRequired(false);

                entity.Property(x => x.Description)
                    .HasMaxLength(1000)
                    .IsRequired(false);

                entity.Property(x => x.AcademicYear)
                    .HasMaxLength(30)
                    .IsRequired(false);

                entity.Property(x => x.MaxStudentCount)
                    .HasDefaultValue(30);

                entity.HasIndex(x => x.Name)
                    .IsUnique();
            });
        }
        private static void ConfigureExamSecurityLog(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExamSecurityLog>(entity =>
            {
                entity.ToTable("ExamSecurityLogs");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.EventType)
                    .IsRequired()
                    .HasConversion<int>();

                entity.Property(x => x.Description)
                    .HasMaxLength(1000)
                    .IsRequired(false);

                entity.Property(x => x.OccurredAt)
                    .IsRequired();

                entity.HasOne(x => x.StudentExam)
                    .WithMany()
                    .HasForeignKey(x => x.StudentExamId)
                    .OnDelete(DeleteBehavior.Cascade);
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

                entity.Property(x => x.Code)
                    .HasMaxLength(50)
                    .IsRequired(false);

                entity.Property(x => x.IsActive)
                    .HasDefaultValue(true);

                entity.Property(x => x.Description)
                    .HasMaxLength(1000)
                    .IsRequired(false);

                entity.Property(x => x.WeeklyHours)
                    .IsRequired()
                    .HasDefaultValue(0);

                entity.HasIndex(x => x.Name)
                    .IsUnique();

                entity.HasIndex(x => x.Code)
                    .IsUnique()
                    .HasFilter("[Code] IS NOT NULL");
            });
        }

        private static void ConfigureTeacherSubject(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeacherSubject>(entity =>
            {
                entity.ToTable("TeacherSubjects");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.IsActive)
                    .HasDefaultValue(true);

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

                entity.Property(x => x.JoinedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(x => x.LeftAt)
                    .IsRequired(false);

                entity.Property(x => x.IsActive)
                    .HasDefaultValue(true);

                entity.HasOne(x => x.Student)
                    .WithMany(x => x.StudentClasses)
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.ClassRoom)
                    .WithMany(x => x.StudentClasses)
                    .HasForeignKey(x => x.ClassRoomId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => new { x.StudentId, x.ClassRoomId })
                    .IsUnique()
                    .HasFilter("[IsActive] = 1");
            });
        }

        private static void ConfigureClassTeacherSubject(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClassTeacherSubject>(entity =>
            {
                entity.ToTable("ClassTeacherSubjects");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.IsActive)
                    .HasDefaultValue(true);

                entity.HasOne(x => x.ClassRoom)
                    .WithMany(x => x.ClassTeacherSubjects)
                    .HasForeignKey(x => x.ClassRoomId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Teacher)
                    .WithMany(x => x.ClassTeacherSubjects)
                    .HasForeignKey(x => x.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Subject)
                    .WithMany(x => x.ClassTeacherSubjects)
                    .HasForeignKey(x => x.SubjectId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.ClassRoomId, x.SubjectId, x.TeacherId })
                    .IsUnique()
                    .HasFilter("[IsActive] = 1");
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

                entity.Property(x => x.Description)
                    .HasMaxLength(2000)
                    .IsRequired(false);

                entity.Property(x => x.TotalScore)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false);

                entity.Property(x => x.IsPublished)
                    .HasDefaultValue(false);

                entity.Property(x => x.ClosedQuestionScore)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired(false);

                entity.Property(x => x.TotalQuestionCount)
                    .HasDefaultValue(0);

                entity.Property(x => x.OpenQuestionCount)
                    .HasDefaultValue(0);

                entity.Property(x => x.ClosedQuestionCount)
                    .HasDefaultValue(0);

                entity.Property(x => x.Instructions)
                    .HasMaxLength(2000)
                    .IsRequired(false);

                entity.HasOne(x => x.Subject)
                    .WithMany(x => x.Exams)
                    .HasForeignKey(x => x.SubjectId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Teacher)
                    .WithMany(x => x.CreatedExams)
                    .HasForeignKey(x => x.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.ClassRoom)
                    .WithMany(x => x.Exams)
                    .HasForeignKey(x => x.ClassRoomId)
                    .OnDelete(DeleteBehavior.SetNull);
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

                entity.Property(x => x.OrderNumber)
                    .HasDefaultValue(0);

                entity.Property(x => x.Description)
                    .HasMaxLength(2000)
                    .IsRequired(false);

                entity.Property(x => x.SelectionMode)
                    .HasConversion<int>()
                    .IsRequired(false);

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

                entity.Property(x => x.OptionKey)
                    .HasMaxLength(10)
                    .IsRequired(false);

                entity.Property(x => x.OrderNumber)
                    .HasDefaultValue(0);

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
                // YENI
                entity.Property(x => x.Status)
                    .IsRequired()
                    .HasConversion<int>()
                    .HasDefaultValue(StudentExamStatus.Pending);

                // YENI
                entity.Property(x => x.IsLocked)
                    .HasDefaultValue(false);

                // YENI
                entity.Property(x => x.WarningCount)
                    .HasDefaultValue(0);

                // YENI
                entity.Property(x => x.TabSwitchCount)
                    .HasDefaultValue(0);

                // YENI
                entity.Property(x => x.FullScreenExitCount)
                    .HasDefaultValue(0);

              
                entity.Property(x => x.IsAutoSubmitted)
                    .HasDefaultValue(false);
                entity.HasOne(x => x.Exam)
                    .WithMany(x => x.StudentExams)
                    .HasForeignKey(x => x.ExamId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.StudentId, x.ExamId })
                    .IsUnique();

                entity.Property(x => x.ReviewedAt)
                    .IsRequired(false);

                entity.Property(x => x.ReviewedByTeacherId)
                    .IsRequired(false);

                entity.Property(x => x.AutoGradedScore)
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);

                entity.Property(x => x.ManualGradedScore)
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);
            });
        }

        private static void ConfigureStudentAnswer(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentAnswer>(entity =>
            {
                entity.Property(x => x.IsCorrect)
                    .IsRequired(false);

                entity.ToTable("StudentAnswers");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.AnswerText)
                    .HasMaxLength(4000);

                entity.Property(x => x.PointsAwarded)
                    .HasColumnType("decimal(18,2)");

                entity.Property(x => x.IsReviewed)
                    .HasDefaultValue(false);

                entity.Property(x => x.ReviewedAt)
                    .IsRequired(false);

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

                entity.Property(x => x.TeacherFeedback)
                    .HasMaxLength(2000)
                    .IsRequired(false);

                entity.HasIndex(x => new { x.StudentExamId, x.ExamQuestionId })
                    .IsUnique();
            });
        }

        private static void ConfigureStudentAnswerOption(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentAnswerOption>(entity =>
            {
                entity.ToTable("StudentAnswerOptions");

                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.StudentAnswer)
                    .WithMany(x => x.SelectedOptions)
                    .HasForeignKey(x => x.StudentAnswerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.ExamOption)
                    .WithMany()
                    .HasForeignKey(x => x.ExamOptionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.StudentAnswerId, x.ExamOptionId })
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
                // YENI
                entity.Property(x => x.GeneratedAt)
                    .IsRequired();

                // YENI
                entity.Property(x => x.UsedAt)
                    .IsRequired(false);

                // YENI
                entity.HasIndex(x => new { x.ExamId, x.StudentId })
                    .IsUnique();

                // YENI
                entity.HasIndex(x => x.AccessCode)
                    .IsUnique();

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
                entity.Property(x => x.Category)
    .IsRequired();

                entity.Property(x => x.Priority)
                    .IsRequired()
                    .HasDefaultValue(NotificationPriority.Medium);

                entity.Property(x => x.ReadAt)
                    .IsRequired(false);

                entity.Property(x => x.RelatedEntityType)
                    .HasMaxLength(100)
                    .IsRequired(false);

                entity.Property(x => x.ActionUrl)
                    .HasMaxLength(500)
                    .IsRequired(false);

                entity.Property(x => x.Icon)
                    .HasMaxLength(100)
                    .IsRequired(false);

                entity.Property(x => x.MetadataJson)
                    .HasColumnType("nvarchar(max)")
                    .IsRequired(false);

                entity.Property(x => x.ExpiresAt)
                    .IsRequired(false);

                entity.HasIndex(x => new { x.UserId, x.IsRead, x.CreatedAt });
                entity.HasIndex(x => new { x.UserId, x.Type, x.Category });
                entity.HasIndex(x => new { x.RelatedEntityType, x.RelatedEntityId });
            });
        }
        private static void ConfigureSystemSetting(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SystemSetting>(entity =>
            {
                entity.ToTable("SystemSettings");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.SystemName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.SchoolName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.AcademicYear)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(x => x.DefaultLanguage)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(x => x.DefaultExamDurationMinutes)
                    .HasDefaultValue(90);

                entity.Property(x => x.AccessCodeActivationMinutes)
                    .HasDefaultValue(5);

                entity.Property(x => x.LateEntryToleranceMinutes)
                    .HasDefaultValue(10);

                entity.Property(x => x.DefaultPassScore)
                    .HasDefaultValue(51);

                entity.Property(x => x.AutoSubmitOnEndTime)
                    .HasDefaultValue(true);

                entity.Property(x => x.AllowEarlySubmit)
                    .HasDefaultValue(true);

                entity.Property(x => x.AutoPublishResults)
                    .HasDefaultValue(false);

                entity.Property(x => x.ShowScoreImmediately)
                    .HasDefaultValue(true);

                entity.Property(x => x.ShowCorrectAnswersAfterCompletion)
                    .HasDefaultValue(false);

                entity.Property(x => x.ExamPublishNotificationEnabled)
                    .HasDefaultValue(true);

                entity.Property(x => x.ExamRescheduleNotificationEnabled)
                    .HasDefaultValue(true);

                entity.Property(x => x.ExamStartNotificationEnabled)
                    .HasDefaultValue(true);

                entity.Property(x => x.ResultNotificationEnabled)
                    .HasDefaultValue(true);

                entity.Property(x => x.AttendanceNotificationEnabled)
                    .HasDefaultValue(true);

                entity.Property(x => x.TaskNotificationEnabled)
                    .HasDefaultValue(true);

                entity.Property(x => x.AutoSaveIntervalSeconds)
                    .HasDefaultValue(30);

                entity.Property(x => x.SessionTimeoutMinutes)
                    .HasDefaultValue(20);

                entity.Property(x => x.MaxAccessCodeAttempts)
                    .HasDefaultValue(5);

                entity.Property(x => x.AllowReEntry)
                    .HasDefaultValue(true);
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

        private static void ConfigureStudentTask(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentTask>(entity =>
            {
                entity.ToTable("StudentTasks");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.TaskGroupKey)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Description)
                    .HasMaxLength(2000)
                    .IsRequired(false);

                entity.Property(x => x.AssignedDate)
                    .IsRequired();

                entity.Property(x => x.DueDate)
                    .IsRequired();

                entity.Property(x => x.Status)
                    .IsRequired()
                    .HasConversion<int>()
                    .HasDefaultValue(StudentTaskStatus.Pending);

                entity.Property(x => x.Score)
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);

                entity.Property(x => x.MaxScore)
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(100);

                entity.Property(x => x.Link)
                    .HasMaxLength(1000)
                    .IsRequired(false);

                entity.Property(x => x.Note)
                    .HasMaxLength(2000)
                    .IsRequired(false);

                entity.Property(x => x.SubmissionText)
                    .HasMaxLength(5000)
                    .IsRequired(false);

                entity.Property(x => x.SubmissionLink)
                    .HasMaxLength(1000)
                    .IsRequired(false);

                entity.Property(x => x.SubmissionFileUrl)
                    .HasMaxLength(1000)
                    .IsRequired(false);

                entity.Property(x => x.Feedback)
                    .HasMaxLength(3000)
                    .IsRequired(false);

                entity.Property(x => x.CheckedAt)
                    .IsRequired(false);

                entity.Property(x => x.SubmittedAt)
                    .IsRequired(false);

                entity.Property(x => x.IsActive)
                    .HasDefaultValue(true);

                entity.HasOne(x => x.Student)
                    .WithMany(x => x.Tasks)
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Subject)
                    .WithMany(x => x.StudentTasks)
                    .HasForeignKey(x => x.SubjectId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Teacher)
                    .WithMany(x => x.StudentTasks)
                    .HasForeignKey(x => x.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.ClassRoom)
                    .WithMany(x => x.StudentTasks)
                    .HasForeignKey(x => x.ClassRoomId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.TaskGroupKey);
                entity.HasIndex(x => new { x.TeacherId, x.ClassRoomId, x.TaskGroupKey });
                entity.HasIndex(x => new { x.StudentId, x.DueDate });
            });
        }

        private static void ConfigureTeacherTask(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeacherTask>(entity =>
            {
                entity.ToTable("TeacherTasks");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Description)
                    .HasMaxLength(2000)
                    .IsRequired(false);

                entity.Property(x => x.DueDate)
                    .IsRequired();

                entity.Property(x => x.Status)
                    .IsRequired()
                    .HasConversion<int>()
                    .HasDefaultValue(TeacherTaskStatus.Waiting);

                entity.Property(x => x.IsCompleted)
                    .HasDefaultValue(false);

                entity.HasOne(x => x.Teacher)
                    .WithMany(x => x.Tasks)
                    .HasForeignKey(x => x.TeacherId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => new { x.TeacherId, x.DueDate });
            });
        }

        // YENI
        private static void ConfigureAttendanceSession(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttendanceSession>(entity =>
            {
                entity.ToTable("AttendanceSessions");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.SessionDate)
                    .IsRequired();

                entity.Property(x => x.StartTime)
                    .IsRequired(false);

                entity.Property(x => x.EndTime)
                    .IsRequired(false);

                entity.Property(x => x.Notes)
                    .HasMaxLength(1000)
                    .IsRequired(false);

                // YENI
                entity.Property(x => x.SessionType)
                    .IsRequired()
                    .HasConversion<int>();

                // YENI
                entity.Property(x => x.IsLocked)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.HasOne(x => x.ClassRoom)
                    .WithMany(x => x.AttendanceSessions)
                    .HasForeignKey(x => x.ClassRoomId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Subject)
                    .WithMany()
                    .HasForeignKey(x => x.SubjectId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Teacher)
                    .WithMany()
                    .HasForeignKey(x => x.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.ClassRoomId, x.SubjectId, x.TeacherId, x.SessionDate })
                    .IsUnique();
            });
        }

        // YENI
        private static void ConfigureAttendanceChangeRequest(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttendanceChangeRequest>(entity =>
            {
                entity.ToTable("AttendanceChangeRequests");

                entity.HasKey(x => x.Id);

                // YENI
                entity.Property(x => x.AttendanceDate)
                    .IsRequired();

                entity.Property(x => x.CurrentStatus)
                    .IsRequired()
                    .HasConversion<int>();

                entity.Property(x => x.RequestedStatus)
                    .IsRequired()
                    .HasConversion<int>();

                // YENI
                entity.Property(x => x.RequestedChangeReason)
                    .HasMaxLength(1000)
                    .IsRequired();

                // YENI
                entity.Property(x => x.RequestedAbsenceReasonType)
                    .HasMaxLength(100)
                    .IsRequired(false);

                // YENI
                entity.Property(x => x.RequestedAbsenceReasonNote)
                    .HasMaxLength(500)
                    .IsRequired(false);

                // YENI
                entity.Property(x => x.RequestedLateArrivalTime)
                    .IsRequired(false);

                // YENI
                entity.Property(x => x.RequestedLateNote)
                    .HasMaxLength(500)
                    .IsRequired(false);

                entity.Property(x => x.RequestedAt)
                    .IsRequired();

                // YENI
                entity.Property(x => x.RequestStatus)
                    .HasMaxLength(50)
                    .IsRequired();

                // YENI
                entity.Property(x => x.ReviewNote)
                    .HasMaxLength(1000)
                    .IsRequired(false);

                entity.Property(x => x.ReviewedByAdminId)
                    .IsRequired(false);

                entity.Property(x => x.ReviewedAt)
                    .IsRequired(false);

                // YENI
                entity.HasOne(x => x.ClassRoom)
                    .WithMany()
                    .HasForeignKey(x => x.ClassRoomId)
                    .OnDelete(DeleteBehavior.Restrict);

                // YENI
                entity.HasOne(x => x.Subject)
                    .WithMany()
                    .HasForeignKey(x => x.SubjectId)
                    .OnDelete(DeleteBehavior.Restrict);

                // YENI
                entity.HasOne(x => x.Teacher)
                    .WithMany()
                    .HasForeignKey(x => x.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Student)
                    .WithMany()
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.RequestedByTeacher)
                    .WithMany()
                    .HasForeignKey(x => x.RequestedByTeacherId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => x.ClassRoomId);
                entity.HasIndex(x => x.SubjectId);
                entity.HasIndex(x => x.TeacherId);
                entity.HasIndex(x => x.StudentId);
                entity.HasIndex(x => x.RequestedByTeacherId);
                entity.HasIndex(x => x.RequestedAt);

                // YENI
                entity.HasIndex(x => new
                {
                    x.ClassRoomId,
                    x.SubjectId,
                    x.StudentId,
                    x.AttendanceDate,
                    x.RequestStatus
                });
            });
        }

        // YENI
        // YENI
        private static void ConfigureRefreshToken(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshTokens");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Token)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(x => x.ExpiresAt)
                    .IsRequired();

                entity.Property(x => x.ReplacedByToken)
                    .HasMaxLength(500)
                    .IsRequired(false);

                entity.Property(x => x.RevocationReason)
                    .HasMaxLength(500)
                    .IsRequired(false);

                entity.HasIndex(x => x.Token)
                    .IsUnique();

                entity.HasIndex(x => x.UserId);
            });
        }
        private static void ConfigureAttendanceRecord(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttendanceRecord>(entity =>
            {
                entity.ToTable("AttendanceRecords");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.AttendanceSessionId)
                    .IsRequired();

                entity.Property(x => x.StudentId)
                    .IsRequired();

                entity.Property(x => x.Status)
                    .IsRequired()
                    .HasConversion<int>();

                entity.Property(x => x.Notes)
                    .HasMaxLength(500)
                    .IsRequired(false);

                entity.Property(x => x.AbsenceReasonType)
                    .HasMaxLength(100)
                    .IsRequired(false);

                // YENI
                entity.Property(x => x.AbsenceReasonNote)
                    .HasMaxLength(500)
                    .IsRequired(false);

                // YENI
                entity.Property(x => x.LateArrivalTime)
                    .IsRequired(false);

                // YENI
                entity.Property(x => x.LateNote)
                    .HasMaxLength(500)
                    .IsRequired(false);

                entity.HasOne(x => x.AttendanceSession)
                    .WithMany(x => x.Records)
                    .HasForeignKey(x => x.AttendanceSessionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Student)
                    .WithMany(x => x.AttendanceRecords)
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.AttendanceSessionId, x.StudentId })
                    .IsUnique();

                // YENI
                entity.HasIndex(x => x.StudentId);

                // YENI
                entity.HasIndex(x => x.Status);
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