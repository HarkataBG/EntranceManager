using EntranceManager.Models;

namespace EntranceManager.Services
{
    public interface IEntranceService
    {
        Task<IEnumerable<Entrance>> GetAllEntrancesAsync();

        Task<Entrance?> GetEntranceByIdAsync(int id);

        Task AddEntranceAsync(Entrance entrance);

        Task UpdateEntranceAsync(Entrance entrance);

        Task DeleteEntranceAsync(int id);
    }
}