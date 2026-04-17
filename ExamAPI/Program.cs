using ExamAPI.Middlewares;
using ExamApplication.Helper;
using ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Repository.ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Services;
using ExamApplication.Options;
using ExamApplication.Services;
using ExamInfrastucture.DAL;
using ExamInfrastucture.Persistence.Repositories;
using ExamInfrastucture.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controller-ləri əlavə edir.
builder.Services.AddControllers();

// CORS üçün allowed origin-ləri config-dən oxuyur.
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (allowedOrigins.Length == 0)
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();

            return;
        }

        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// appsettings.json içindən connection string-i oxuyur.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Connection string tapılmazsa tətbiqi dayandırır.
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception("DefaultConnection tapılmadı");
}

// Entity Framework DbContext qeydiyyatı.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// HttpContext üzərindən cari istifadəçi məlumatlarını oxumaq üçün lazımdır.
builder.Services.AddHttpContextAccessor();

// Jwt section-u configuration-dan oxuyur.
var jwtSection = builder.Configuration.GetSection("Jwt");

// JwtSettings class-ına bind edir.
builder.Services.Configure<JwtSettings>(jwtSection);

// JWT dəyərlərini ayrıca oxuyur.
var jwtKey = jwtSection["Key"];
var jwtIssuer = jwtSection["Issuer"];
var jwtAudience = jwtSection["Audience"];

// JWT Key boşdursa tətbiqi dayandırır.
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new Exception("Jwt:Key tapılmadı");
}

// JWT Issuer boşdursa tətbiqi dayandırır.
if (string.IsNullOrWhiteSpace(jwtIssuer))
{
    throw new Exception("Jwt:Issuer tapılmadı");
}

// JWT Audience boşdursa tətbiqi dayandırır.
if (string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new Exception("Jwt:Audience tapılmadı");
}

// Symmetric security key yaradır.
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

// JWT authentication qeydiyyatı.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,

            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Email settings config
var emailSection = builder.Configuration.GetSection("EmailSettings");
builder.Services.Configure<EmailSettings>(emailSection);

// Authorization əlavə edir.
builder.Services.AddAuthorization();

// Repository qeydiyyatları
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<IClassRoomRepository, ClassRoomRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ITeacherSubjectRepository, TeacherSubjectRepository>();
builder.Services.AddScoped<IStudentClassRepository, StudentClassRepository>();
builder.Services.AddScoped<IClassTeacherSubjectRepository, ClassTeacherSubjectRepository>();
builder.Services.AddScoped<IExamRepository, ExamRepository>();
builder.Services.AddScoped<IExamQuestionRepository, ExamQuestionRepository>();
builder.Services.AddScoped<IExamOptionRepository, ExamOptionRepository>();
builder.Services.AddScoped<IStudentExamRepository, StudentExamRepository>();
builder.Services.AddScoped<IStudentAnswerRepository, StudentAnswerRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IStudentAnswerOptionRepository, StudentAnswerOptionRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IExamAccessCodeRepository, ExamAccessCodeRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<ITeacherTaskRepository, TeacherTaskRepository>();
builder.Services.AddScoped<IAttendanceChangeRequestRepository, AttendanceChangeRequestRepository>();
builder.Services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();
builder.Services.AddScoped<IAttendanceSessionRepository, AttendanceSessionRepository>();
builder.Services.AddScoped<IAttendanceRecordRepository, AttendanceRecordRepository>();
builder.Services.AddScoped<IStudentTaskRepository, StudentTaskRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStudentTaskService, StudentTaskService>();
builder.Services.AddScoped<IExamSecurityLogRepository, ExamSecurityLogRepository>();
builder.Services.AddScoped<IStudentProfileService, StudentProfileService>();
builder.Services.AddScoped<IStudentExamService, StudentExamService>();
builder.Services.AddScoped<IExamAccessCodeService, ExamAccessCodeService>();
builder.Services.AddHostedService<ExamLifecycleHostedService>();
builder.Services.AddScoped<ISystemSettingService, SystemSettingService>();
builder.Services.AddScoped<IStudentAdminService, StudentAdminService>();
builder.Services.AddScoped<ITeacherTaskManagementService, TeacherTaskManagementService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IClassRoomService, ClassRoomService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Exam API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Bearer {token} formatında JWT token daxil edin",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

var enableSwaggerInProduction = builder.Configuration.GetValue<bool>("Swagger:EnableInProduction");

// Development və ya config icazə verirsə Swagger açılır.
if (app.Environment.IsDevelopment() || enableSwaggerInProduction)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

// CORS policy tətbiq olunur.
app.UseCors("AllowFrontend");

// Authentication mütləq Authorization-dan əvvəl gəlməlidir.
app.UseAuthentication();
app.UseAuthorization();

// Controller routelarını map edir.
app.MapControllers();

// Migration + seed
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    await context.Database.MigrateAsync();
    await ExamInfrastucture.Seed.DataSeeder.SeedAsync(context, passwordHasher, configuration);
}

// Tətbiqi işə salır.
app.Run();