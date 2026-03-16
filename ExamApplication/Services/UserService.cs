using ExamApplication.DTO.User;
using ExamApplication.Interfaces.Repository;
using ExamApplication.Interfaces.Services;
using ExamDomain.Entities;
using ExamDomain.Enum;

namespace ExamApplication.Services
{
    /// <summary>
    /// Sistem daxilində user account-larının idarə olunmasına cavabdeh olan servisdir.
    /// SuperAdmin, Admin, Teacher və Student rollarına aid əsas hesab əməliyyatları burada idarə olunur.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Yeni user yaradır.
        /// Bu metod əsas account məlumatlarını yaradır və rolu təyin edir.
        /// Teacher və Student profil hissəsi ayrıca servis tərəfindən tamamlana bilər.
        /// </summary>
        /// <param name="request">Yeni user yaratmaq üçün göndərilən məlumatlar</param>
        /// <param name="cancellationToken">Async əməliyyatı dayandırmaq üçün token</param>
        /// <returns>Yaradılmış user-in detail məlumatları</returns>
        public async Task<UserDetailDto> CreateAsync(
            CreateUserRequestDto request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Username))
                throw new Exception("Username boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new Exception("Email boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new Exception("Şifrə boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(request.Role))
                throw new Exception("Rol boş ola bilməz.");

            var normalizedUsername = request.Username.Trim();
            var normalizedEmail = request.Email.Trim().ToLower();
            var parsedRole = ParseUserRole(request.Role);

            var usernameExists = await _unitOfWork.Users.ExistsByUsernameAsync(
                normalizedUsername,
                cancellationToken);

            if (usernameExists)
                throw new Exception("Bu username artıq mövcuddur.");

            var emailExists = await _unitOfWork.Users.ExistsByEmailAsync(
                normalizedEmail,
                cancellationToken);

            if (emailExists)
                throw new Exception("Bu email artıq mövcuddur.");

            var user = new User
            {
                Username = normalizedUsername,
                Email = normalizedEmail,
                PasswordHash = _passwordHasher.Hash(request.Password),
                Role = parsedRole,
                IsActive = request.IsActive
            };

            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var createdUser = await _unitOfWork.Users.GetByIdWithDetailsAsync(user.Id, cancellationToken);

            if (createdUser == null)
                throw new Exception("Yaradılmış istifadəçi tapılmadı.");

            return MapToDetailDto(createdUser);
        }

        /// <summary>
        /// Sistemdə olan bütün user-ləri qaytarır.
        /// Admin paneldə ümumi user siyahısını göstərmək üçün istifadə olunur.
        /// </summary>
        /// <param name="cancellationToken">Async əməliyyatı dayandırmaq üçün token</param>
        /// <returns>User siyahısı</returns>
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

        /// <summary>
        /// Verilmiş role-a uyğun olan user-ləri qaytarır.
        /// Məsələn yalnız SuperAdmin, yalnız Admin, yalnız Teacher və ya yalnız Student user-ləri filtr etmək üçün istifadə olunur.
        /// </summary>
        /// <param name="role">Filter üçün role adı</param>
        /// <param name="cancellationToken">Async əməliyyatı dayandırmaq üçün token</param>
        /// <returns>Verilmiş role-a uyğun user siyahısı</returns>
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

        /// <summary>
        /// Verilmiş Id-yə görə user-in detail məlumatını qaytarır.
        /// </summary>
        /// <param name="userId">Axtarılan user-in Id-si</param>
        /// <param name="cancellationToken">Async əməliyyatı dayandırmaq üçün token</param>
        /// <returns>User detail məlumatı</returns>
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

        /// <summary>
        /// Mövcud user-in əsas hesab məlumatlarını yeniləyir.
        /// Username, email və role burada dəyişdirilə bilər.
        /// </summary>
        /// <param name="request">Yenilənəcək user məlumatları</param>
        /// <param name="cancellationToken">Async əməliyyatı dayandırmaq üçün token</param>
        /// <returns>Yenilənmiş user detail məlumatı</returns>
        public async Task<UserDetailDto> UpdateAsync(
            UpdateUserRequestDto request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.UserId <= 0)
                throw new Exception("User Id düzgün deyil.");

            if (string.IsNullOrWhiteSpace(request.Username))
                throw new Exception("Username boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new Exception("Email boş ola bilməz.");

            if (string.IsNullOrWhiteSpace(request.Role))
                throw new Exception("Rol boş ola bilməz.");

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
                throw new Exception("İstifadəçi tapılmadı.");

            var normalizedUsername = request.Username.Trim();
            var normalizedEmail = request.Email.Trim().ToLower();
            var parsedRole = ParseUserRole(request.Role);

            var allUsers = await _unitOfWork.Users.GetAllAsync(cancellationToken);

            var usernameExistsOnAnotherUser = allUsers.Any(x =>
                x.Id != request.UserId &&
                x.Username.ToLower() == normalizedUsername.ToLower());

            if (usernameExistsOnAnotherUser)
                throw new Exception("Bu username artıq başqa istifadəçi tərəfindən istifadə olunur.");

            var emailExistsOnAnotherUser = allUsers.Any(x =>
                x.Id != request.UserId &&
                x.Email.ToLower() == normalizedEmail.ToLower());

            if (emailExistsOnAnotherUser)
                throw new Exception("Bu email artıq başqa istifadəçi tərəfindən istifadə olunur.");

            user.Username = normalizedUsername;
            user.Email = normalizedEmail;
            user.Role = parsedRole;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updatedUser = await _unitOfWork.Users.GetByIdWithDetailsAsync(user.Id, cancellationToken);

            if (updatedUser == null)
                throw new Exception("Yenilənmiş istifadəçi tapılmadı.");

            return MapToDetailDto(updatedUser);
        }

        /// <summary>
        /// User-in aktiv/passiv statusunu dəyişir.
        /// Bu metod soft delete və ya user bloklama məqsədi ilə istifadə oluna bilər.
        /// </summary>
        /// <param name="request">Status dəyişmə məlumatları</param>
        /// <param name="cancellationToken">Async əməliyyatı dayandırmaq üçün token</param>
        public async Task ChangeStatusAsync(
            ChangeUserStatusRequestDto request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.UserId <= 0)
                throw new Exception("User Id düzgün deyil.");

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
                throw new Exception("İstifadəçi tapılmadı.");

            user.IsActive = request.IsActive;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Verilmiş Id-yə görə user-i sistemdən silir.
        /// Real sistemdə bu əməliyyat çox vaxt soft delete və ya deactivate kimi implement olunur.
        /// Burada təhlükəsiz yanaşma olaraq user passiv edilir.
        /// </summary>
        /// <param name="userId">Silinəcək user-in Id-si</param>
        /// <param name="cancellationToken">Async əməliyyatı dayandırmaq üçün token</param>
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

        /// <summary>
        /// Verilmiş rol mətnini UserRole enum-na çevirir.
        /// </summary>
        /// <param name="role">Rol mətni</param>
        /// <returns>UserRole enum dəyəri</returns>
        private static UserRole ParseUserRole(string role)
        {
            if (Enum.TryParse<UserRole>(role, true, out var parsedRole))
                return parsedRole;

            throw new Exception("Göndərilən rol dəyəri düzgün deyil.");
        }

        /// <summary>
        /// User entity-ni siyahı üçün qısa dto modelinə çevirir.
        /// </summary>
        /// <param name="user">Çevriləcək user entity-si</param>
        /// <returns>UserListItemDto</returns>
        private static UserListItemDto MapToListItemDto(User user)
        {
            return new UserListItemDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                FullName = GetUserFullName(user),
                CreatedAt = user.CreatedAt
            };
        }

        /// <summary>
        /// User entity-ni detail dto modelinə çevirir.
        /// </summary>
        /// <param name="user">Çevriləcək user entity-si</param>
        /// <returns>UserDetailDto</returns>
        private static UserDetailDto MapToDetailDto(User user)
        {
            return new UserDetailDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                FullName = GetUserFullName(user),
                TeacherId = user.Teacher?.Id,
                StudentId = user.Student?.Id,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        /// <summary>
        /// User üçün göstəriləcək tam adı qaytarır.
        /// Əgər student və ya teacher profili varsa oradan götürülür,
        /// yoxdursa username qaytarılır.
        /// </summary>
        /// <param name="user">Tam adı tapılacaq user</param>
        /// <returns>Display full name</returns>
        private static string GetUserFullName(User user)
        {
            if (user.Student != null && !string.IsNullOrWhiteSpace(user.Student.FullName))
                return user.Student.FullName;

            if (user.Teacher != null && !string.IsNullOrWhiteSpace(user.Teacher.FullName))
                return user.Teacher.FullName;

            return user.Username;
        }
    }
}