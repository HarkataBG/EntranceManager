using EntranceManager.Exceptions;
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
                       ?? throw new UserNotFoundException(userId);

            var entrance = await _entranceRepository.GetEntranceByIdAsync(entranceId, false)
                           ?? throw new EntranceNotFoundException(entranceId);

            entrance.ManagerUserId = user.Id;

            user.Role = nameof(UserRole.EntranceManager);

            await _userRepository.UpdateAsync(user);
            await _entranceRepository.UpdateAsync(entrance);
        }

        public async Task DemoteFromManagerAsync(int entranceId)
        {
            var entrance = await _entranceRepository.GetEntranceByIdAsync(entranceId, false)
                           ?? throw new EntranceNotFoundException(entranceId);

            if (entrance.ManagerUserId == null)
                throw new ManagerNotFoundException(entranceId);

            var managerUser = await _userRepository.GetByIdAsync(entrance.ManagerUserId.Value);
            if (managerUser == null)
                throw new UserNotFoundException(entrance.ManagerUserId.Value);

            entrance.ManagerUserId = null;
            await _entranceRepository.UpdateAsync(entrance);

            bool isManagerElsewhere = await _entranceRepository
            .AnyManagedEntrancesAsync(managerUser.Id, excludeEntranceId: entranceId);

            if (!isManagerElsewhere && managerUser.Role == nameof(UserRole.EntranceManager))
            {
                managerUser.Role = nameof(UserRole.User); 
                await _userRepository.UpdateAsync(managerUser);
            }
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
                    throw new UnauthorizedAccessException("You cannot manage apartments outside your assigned entrances.");
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
