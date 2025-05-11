using EntranceManager.Models;
using EntranceManager.Repositories;

namespace EntranceManager.Services
{
    public class UsersService
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
            user.Role = "Manager"; // Или друга стойност според бизнес логиката
            await _userRepository.UpdateAsync(user);
        }
    }
}
