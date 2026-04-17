using ExamApplication.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserDetailDto> CreateAsync(
            CreateUserRequestDto request,
            CancellationToken cancellationToken = default);

        Task<List<UserListItemDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<List<UserListItemDto>> GetByRoleAsync(
            string role,
            CancellationToken cancellationToken = default);

        Task<UserDetailDto> GetByIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        Task<UserDetailDto> UpdateAsync(
            UpdateUserRequestDto request,
            CancellationToken cancellationToken = default);

        Task ChangeStatusAsync(
            ChangeUserStatusRequestDto request,
            int currentUserId,
            string currentUserRole,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            int userId,
            CancellationToken cancellationToken = default);
    }
}
