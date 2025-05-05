using EntranceManager.Models;

namespace EntranceManager.Repositories
{
    public interface IEntranceRepository
    {
        Task<IEnumerable<Entrance>> GetAllAsync();

        Task<Entrance?> GetByIdAsync(int id);

        Task AddAsync(Entrance entrance);

        Task UpdateAsync(Entrance entrance);

        Task DeleteAsync(int id);
    }
}