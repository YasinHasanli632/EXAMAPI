using ExamApplication.DTO.Notification;
using ExamApplication.DTO.User;
using ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Services;
using ExamDomain.Entities;
using ExamDomain.Enum;

namespace ExamApplication.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly INotificationService _notificationService; // YENI
        public UserService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _notificationService = notificationService;
        }

        public async Task<UserDetailDto> CreateAsync(
      CreateUserRequestDto request,
      CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            ValidateCreateRequest(request);

            var normalizedUsername = NormalizeUsername(request.Username);
            var normalizedEmail = NormalizeEmail(request.Email);
            var parsedRole = ParseUserRole(request.Role);

            await EnsureUsernameUniqueAsync(normalizedUsername, null, cancellationToken);
            await EnsureEmailUniqueAsync(normalizedEmail, null, cancellationToken);

            var user = new User
            {
                Username = normalizedUsername,
                Email = normalizedEmail,
                PasswordHash = _passwordHasher.Hash(request.Password.Trim()),

                // YENI - ilk girişdə şifrə dəyişmə məcburi olsun
                MustChangePassword = true,

                Role = parsedRole,
                IsActive = request.IsActive,
                FirstName = request.FirstName?.Trim() ?? string.Empty,
                LastName = request.LastName?.Trim() ?? string.Empty,
                FatherName = request.FatherName?.Trim() ?? string.Empty,
                BirthDate = request.BirthDate,
                PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim(),
                Country = string.IsNullOrWhiteSpace(request.Country) ? null : request.Country.Trim(),
                PhotoUrl = string.IsNullOrWhiteSpace(request.PhotoUrl) ? null : request.PhotoUrl.Trim(),
                Details = string.IsNullOrWhiteSpace(request.Details) ? null : request.Details.Trim(),

                // YENI - forgot password üçün başlanğıc dəyərlər
                PasswordResetOtp = null,
                PasswordResetOtpExpiresAt = null,
                PasswordResetOtpUsed = false,
                PasswordResetOtpAttemptCount = 0,
                PasswordResetRequestedAt = null
            };

            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var createdUser = await _unitOfWork.Users.GetByIdWithDetailsAsync(user.Id, cancellationToken);

            if (createdUser == null)
                throw new Exception("Yaradılmış istifadəçi tapılmadı.");

            return MapToDetailDto(createdUser);
        }

        public async Task<List<UserListItemDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);

            return users
                .Select(MapToListItemDto)
                .OrderBy(x => x.Role)
                .ThenBy(x => x.Username)
                .ToList();
        }

        public async Task<List<UserListItemDto>> GetByRoleAsync(
            string role,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new Exception("Rol boş ola bilməz.");

            var parsedRole = ParseUserRole(role);
            var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);

            return users
                .Where(x => x.Role == parsedRole)
                .Select(MapToListItemDto)
                .OrderBy(x => x.Username)
                .ToList();
        }

        public async Task<UserDetailDto> GetByIdAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            if (userId <= 0)
                throw new Exception("User Id düzgün deyil.");

            var user = await _unitOfWork.Users.GetByIdWithDetailsAsync(userId, cancellationToken);

            if (user == null)
                throw new Exception("İstifadəçi tapılmadı.");

            return MapToDetailDto(user);
        }

        public async Task<UserDetailDto> UpdateAsync(
            UpdateUserRequestDto request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            ValidateUpdateRequest(request);

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
                throw new Exception("İstifadəçi tapılmadı.");

            var normalizedUsername = NormalizeUsername(request.Username);
            var normalizedEmail = NormalizeEmail(request.Email);
            var parsedRole = ParseUserRole(request.Role);

            await EnsureUsernameUniqueAsync(normalizedUsername, request.UserId, cancellationToken);
            await EnsureEmailUniqueAsync(normalizedEmail, request.UserId, cancellationToken);

            user.Username = normalizedUsername;
            user.Email = normalizedEmail;
            user.Role = parsedRole;
            user.FirstName = request.FirstName?.Trim() ?? string.Empty;
            user.LastName = request.LastName?.Trim() ?? string.Empty;
            user.FatherName = request.FatherName?.Trim() ?? string.Empty;
            user.BirthDate = request.BirthDate;
            user.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();
            user.Country = string.IsNullOrWhiteSpace(request.Country) ? null : request.Country.Trim();
            user.PhotoUrl = string.IsNullOrWhiteSpace(request.PhotoUrl) ? null : request.PhotoUrl.Trim();
            user.Details = string.IsNullOrWhiteSpace(request.Details) ? null : request.Details.Trim();

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updatedUser = await _unitOfWork.Users.GetByIdWithDetailsAsync(user.Id, cancellationToken);

            if (updatedUser == null)
                throw new Exception("Yenilənmiş istifadəçi tapılmadı.");

            return MapToDetailDto(updatedUser);
        }

        public async Task ChangeStatusAsync(
            ChangeUserStatusRequestDto request,
            int currentUserId,
            string currentUserRole,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.UserId <= 0)
                throw new Exception("User Id düzgün deyil.");

            if (!string.Equals(currentUserRole, "IsSuperAdmin", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Yalnız SuperAdmin istifadəçi statusunu dəyişə bilər.");

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
                throw new Exception("İstifadəçi tapılmadı.");

            if (user.Role == UserRole.IsSuperAdmin)
                throw new Exception("SuperAdmin istifadəçisini deaktiv etmək olmaz.");

            if (user.Id == currentUserId)
                throw new Exception("Öz hesabınızı deaktiv etmək olmaz.");

            user.IsActive = request.IsActive;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            await _notificationService.CreateAsync(new CreateNotificationDto
            {
                UserId = user.Id,
                Title = "Hesab statusunuz dəyişdirildi",
                Message = request.IsActive
                    ? "Hesabınız aktiv edildi."
                    : "Hesabınız deaktiv edildi.",
                Type = (int)NotificationType.User,
                Category = (int)NotificationCategory.UserStatusChanged,
                Priority = (int)NotificationPriority.High,
                RelatedEntityType = "User",
                RelatedEntityId = user.Id,
                ActionUrl = "/profile",
                Icon = "user-status",
                MetadataJson = $@"{{""userId"":{user.Id},""isActive"":{request.IsActive.ToString().ToLower()}}}",
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            }, cancellationToken);
        }

        public async Task DeleteAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            if (userId <= 0)
                throw new Exception("User Id düzgün deyil.");

            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);

            if (user == null)
                throw new Exception("İstifadəçi tapılmadı.");

            user.IsActive = false;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private static void ValidateCreateRequest(CreateUserRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                throw new Exception("Username boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new Exception("Email boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new Exception("Şifrə boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(request.Role))
                throw new Exception("Rol boş ola bilməz.");
        }

        private static void ValidateUpdateRequest(UpdateUserRequestDto request)
        {
            if (request.UserId <= 0)
                throw new Exception("User Id düzgün deyil.");

            if (string.IsNullOrWhiteSpace(request.Username))
                throw new Exception("Username boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new Exception("Email boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(request.Role))
                throw new Exception("Rol boş ola bilməz.");
        }

        private static string NormalizeUsername(string username)
        {
            return username.Trim();
        }

        private static string NormalizeEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }

        private async Task EnsureUsernameUniqueAsync(
            string username,
            int? ignoreUserId,
            CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);

            var exists = users.Any(x =>
                x.Username.ToLower() == username.ToLower() &&
                (!ignoreUserId.HasValue || x.Id != ignoreUserId.Value));

            if (exists)
                throw new Exception("Bu username artıq mövcuddur.");
        }

        private async Task EnsureEmailUniqueAsync(
            string email,
            int? ignoreUserId,
            CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);

            var exists = users.Any(x =>
                x.Email.ToLower() == email.ToLower() &&
                (!ignoreUserId.HasValue || x.Id != ignoreUserId.Value));

            if (exists)
                throw new Exception("Bu email artıq mövcuddur.");
        }

        private static UserRole ParseUserRole(string role)
        {
            if (Enum.TryParse<UserRole>(role, true, out var parsedRole))
                return parsedRole;

            throw new Exception("Göndərilən rol dəyəri düzgün deyil.");
        }

        private static UserListItemDto MapToListItemDto(User user)
        {
            return new UserListItemDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FatherName = user.FatherName,
                FullName = GetUserFullName(user),
                Age = CalculateAge(user.BirthDate),
                PhoneNumber = user.PhoneNumber,
                Country = user.Country,
                PhotoUrl = user.PhotoUrl,
                CreatedAt = user.CreatedAt
            };
        }

        private static UserDetailDto MapToDetailDto(User user)
        {
            return new UserDetailDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FatherName = user.FatherName,
                FullName = GetUserFullName(user),
                BirthDate = user.BirthDate,
                Age = CalculateAge(user.BirthDate),
                PhoneNumber = user.PhoneNumber,
                Country = user.Country,
                PhotoUrl = user.PhotoUrl,
                Details = user.Details,
                TeacherId = user.Teacher?.Id,
                StudentId = user.Student?.Id,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        private static string GetUserFullName(User user)
        {
            var fullName = string.Join(" ", new[]
            {
                user.FirstName?.Trim(),
                user.LastName?.Trim()
            }.Where(x => !string.IsNullOrWhiteSpace(x)));

            if (!string.IsNullOrWhiteSpace(fullName))
                return fullName;

            if (user.Student != null && !string.IsNullOrWhiteSpace(user.Student.FullName))
                return user.Student.FullName;

            if (user.Teacher != null && !string.IsNullOrWhiteSpace(user.Teacher.FullName))
                return user.Teacher.FullName;

            return user.Username;
        }

        private static int? CalculateAge(DateTime? birthDate)
        {
            if (!birthDate.HasValue)
                return null;

            var today = DateTime.UtcNow.Date;
            var age = today.Year - birthDate.Value.Year;

            if (birthDate.Value.Date > today.AddYears(-age))
                age--;

            return age;
        }
    }
}