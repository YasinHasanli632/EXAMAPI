using ExamApplication.Helper;
using ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Services;
using ExamApplication.Services;
using ExamInfrastucture.DAL;
using ExamInfrastucture.Persistence;
using ExamInfrastucture.Persistence.Repositories;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controller-ləri əlavə edir.
builder.Services.AddControllers();

// Angular və ya başqa front-ların qoşulması üçün CORS açır.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Connection string-i configuration-dan oxuyur.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
    throw new Exception("DefaultConnection tapılmadı");

// DbContext qeydiyyatı.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// HttpContext üzərindən cari user məlumatını oxumaq üçün lazımdır.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IExamAccessCodeRepository, ExamAccessCodeRepository>();
// Repository və UnitOfWork qeydiyyatı.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<IClassRoomRepository, ClassRoomRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ITeacherSubjectRepository, TeacherSubjectRepository>();
builder.Services.AddScoped<IStudentClassRepository, StudentClassRepository>();
builder.Services.AddScoped<IExamRepository, ExamRepository>();
builder.Services.AddScoped<IExamQuestionRepository, ExamQuestionRepository>();
builder.Services.AddScoped<IExamOptionRepository, ExamOptionRepository>();
builder.Services.AddScoped<IStudentExamRepository, StudentExamRepository>();
builder.Services.AddScoped<IStudentAnswerRepository, StudentAnswerRepository>();
builder.Services.AddScoped<IExamAccessCodeRepository, ExamAccessCodeRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<ITeacherClassRoomRepository, TeacherClassRoomRepository>();

// Service qeydiyyatları.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IClassRoomService, ClassRoomService>();

// Köməkçi servis qeydiyyatları.
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// JWT ayarlarını configuration-dan oxuyur.
var jwtSection = builder.Configuration.GetSection("Jwt");

var jwtKey = jwtSection["Key"];
var jwtIssuer = jwtSection["Issuer"];
var jwtAudience = jwtSection["Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
    throw new Exception("Jwt:Key tapılmadı");

if (string.IsNullOrWhiteSpace(jwtIssuer))
    throw new Exception("Jwt:Issuer tapılmadı");

if (string.IsNullOrWhiteSpace(jwtAudience))
    throw new Exception("Jwt:Audience tapılmadı");

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

// JWT authentication qeydiyyatı.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Token imzasını yoxlayır.
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,

            // Token issuer dəyərini yoxlayır.
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            // Token audience dəyərini yoxlayır.
            ValidateAudience = true,
            ValidAudience = jwtAudience,

            // Token expiry vaxtını yoxlayır.
            ValidateLifetime = true,

            // Saat fərqlərinə görə əlavə tolerance verir.
            ClockSkew = TimeSpan.Zero
        };
    });

// Authorization əlavə edir.
builder.Services.AddAuthorization();

// Swagger əlavə edir.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Exam API",
        Version = "v1"
    });

    // Swagger-da bearer token göndərmək üçün security definition əlavə edir.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Bearer {token} formatında JWT token daxil edin",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // Swagger endpoint-lərində authorization tətbiq etmək üçün security requirement əlavə edir.
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

// Development mühitində swagger-i aktiv edir.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS redirect aktiv edir.
app.UseHttpsRedirection();

// CORS tətbiq edir.
app.UseCors("AllowAll");

// Authentication mütləq Authorization-dan əvvəl gəlməlidir.
app.UseAuthentication();
app.UseAuthorization();

// Controller-ləri map edir.
app.MapControllers();

app.Run();