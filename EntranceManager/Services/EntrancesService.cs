using EntranceManager.Models;
using EntranceManager.Repositories;

namespace EntranceManager.Services
{
    public class EntranceService : IEntranceService
    {
        private readonly IEntranceRepository _entranceRepository;

        public EntranceService(IEntranceRepository entranceRepository)
        {
            _entranceRepository = entranceRepository;
        }

        public async Task<IEnumerable<Entrance>> GetAllEntrancesAsync()
        {
            return await _entranceRepository.GetAllAsync();
        }

        public async Task<Entrance?> GetEntranceByIdAsync(int id)
        {
            return await _entranceRepository.GetByIdAsync(id);
        }

        public async Task AddEntranceAsync(Entrance entrance)
        {
            await _entranceRepository.AddAsync(entrance);
        }

        public async Task UpdateEntranceAsync(Entrance entrance)
        {
            await _entranceRepository.UpdateAsync(entrance);
        }

        public async Task DeleteEntranceAsync(int id)
        {
            await _entranceRepository.DeleteAsync(id);
        }
    }
}