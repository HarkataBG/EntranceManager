using EntranceManager.Models;
using EntranceManager.Repositories;
using EntranceManager.Services.Contracts;
using System.Security.Claims;

namespace EntranceManager.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEntranceRepository _entranceRepository;

        public UsersService(IUserRepository userRepository, IEntranceRepository entranceRepository)
        {
            _userRepository = userRepository;
            _entranceRepository = entranceRepository;
        }

        public async Task PromoteToManagerAsync(int userId, int entranceId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                       ?? throw new Exception("User not found.");

            var entrance = await _entranceRepository.GetByIdAsync(entranceId)
                           ?? throw new Exception("Entrance not found.");


            var entranceUser = await _entranceRepository.GetEntranceUserAsync(userId, entranceId);
            if (entranceUser == null)
                throw new UnauthorizedAccessException("Owner must be a resident of the entrance.");

            entrance.ManagerUserId = user.Id;

            await _userRepository.UpdateAsync(user);
            await _entranceRepository.UpdateAsync(entrance);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                throw new Exception($"User with username '{username}' not found.");
            }

            return user;
        }

        public async Task GetAuthorizedUserForEntranceAsync(ClaimsPrincipal userPrincipal, int entranceId)
        {
            var username = userPrincipal.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("User is not authenticated.");

            var user = await GetByUsernameAsync(username);

            if (user.Role != nameof(UserRole.Administrator))
            {
                if (user.Role == "EntranceManager" &&
                    !user.ManagedEntrances.Any(e => e.Id == entranceId))
                {
                    throw new UnauthorizedAccessException("You cannot manage apartments outside your assigned entrance.");
                }
            }
        }

        public async Task GetAuthorizedUserForApartmentAsync(ClaimsPrincipal userPrincipal, Apartment apartment)
        {
            var username = userPrincipal.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("User is not authenticated.");

            var currentUser = await GetByUsernameAsync(username);

            if (currentUser.Role != nameof(UserRole.Administrator))
            {
                if (currentUser.Role == "EntranceManager" &&
                    !currentUser.ManagedEntrances.Any(e => e.Id == apartment.EntranceId))
                {
                    throw new UnauthorizedAccessException("You are not allowed to modify apartments outside your managed entrances.");
                }

                if (currentUser.Role == "User" &&
                    apartment.OwnerUserId != currentUser.Id)
                {
                    throw new UnauthorizedAccessException("You are not allowed to modify apartments you don't own.");
                }
            }
        }
    }
}
