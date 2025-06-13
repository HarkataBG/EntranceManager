using EntranceManager.Models;
using EntranceManager.Models.Mappers;

namespace EntranceManager.Repositories
{
    public interface IEntranceRepository
    {
        Task<IEnumerable<Entrance>> GetAllAsync();

        Task<Entrance?> GetByIdAsync(int id);

        Task<Entrance?> GetEntranceByNameAndAdress(string entranceName, string address);

        Task AddAsync(Entrance entrance);

        Task UpdateAsync(Entrance entrance);

        Task DeleteAsync(int id);

        Task<List<Entrance>> GetAllWithDetailsAsync();

        Task<Entrance> GetWithDetailsByIdAsync(int id);

        Task AddUserToEntranceAsync(EntranceUser entranceUser);

        Task<EntranceUser> GetEntranceUserAsync(int userId, int entranceId);

        Task RemoveUserFromEntranceAsync(EntranceUser apartmentUser);

        Task<bool> AnyManagedEntrancesAsync(int userId, int excludeEntranceId);
    }
}