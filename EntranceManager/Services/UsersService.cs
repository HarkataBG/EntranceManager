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

            bool belongsToEntrance = user.EntranceUsers
            .Any(eu => eu.EntranceId == entrance.Id);

            if (!belongsToEntrance)
            {
                throw new Exception("User does not belong to the specified entrance.");
            }

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

            if (user.Role != "Administrator")
            {
                if (user.Role == "EntranceManager" &&
                    !user.ManagedEntrances.Any(e => e.Id == entranceId))
                {
                    throw new UnauthorizedAccessException("You cannot manage apartments outside your assigned entrance.");
                }
            }

            }
        }
}
