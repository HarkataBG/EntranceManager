using AspNetCoreDemo.Helpers;
using EntranceManager.Data;
using EntranceManager.Models;
using EntranceManager.Models.Mappers;
using Microsoft.EntityFrameworkCore;

namespace EntranceManager.Repositories
{
    public class EntranceRepository : IEntranceRepository
    {
        private readonly ApplicationContext _dbContext;

        public EntranceRepository(ApplicationContext context)
        {
            _dbContext = context;
        }

        public async Task<IEnumerable<Entrance>> GetAllAsync()
        {
            return await _dbContext.Entrances.ToListAsync();
        }

        public async Task<List<Entrance>> GetAllWithDetailsAsync()
        {
            return await _dbContext.Entrances
                  .Include(a => a.EntranceUsers)
                     .ThenInclude(au => au.User)
                 .Include(a => a.Apartments)
                 .ToListAsync();
        }

        public async Task<Entrance> GetWithDetailsByIdAsync(int id)
        {
            return await _dbContext.Entrances
                  .Include(a => a.EntranceUsers)
                     .ThenInclude(au => au.User)
                 .Include(a => a.Apartments)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Entrance?> GetByIdAsync(int id)
        {
            return await _dbContext.Entrances
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Entrance?> GetEntranceByNameAndAdress(string entranceName, string address)
        {
            return await _dbContext.Entrances
                        .FirstOrDefaultAsync(e =>
                            e.EntranceName.ToLower() == entranceName.ToLower() &&
                            e.Address.ToLower() == address.ToLower());

        }

        public async Task AddAsync(Entrance entrance)
        {
            _dbContext.Entrances.Add(entrance);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Entrance entrance)
        {
            _dbContext.Entrances.Update(entrance);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entrance = await _dbContext.Entrances.FindAsync(id);
            if (entrance != null)
            {
                _dbContext.Entrances.Remove(entrance);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task AddUserToEntranceAsync(EntranceUser entranceUser)
        {
            await _dbContext.EntranceUsers.AddAsync(entranceUser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<EntranceUser> GetEntranceUserAsync(int userId, int entranceId)
        {
            return await _dbContext.EntranceUsers
                 .FirstOrDefaultAsync(au => au.UserId == userId && au.EntranceId == entranceId);
        }

        public async Task RemoveUserFromEntranceAsync(EntranceUser entranceUser)
        {
            _dbContext.EntranceUsers.Remove(entranceUser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> AnyManagedEntrancesAsync(int userId, int excludeEntranceId)
        {
            return await _dbContext.Entrances
                .AnyAsync(e => e.ManagerUserId == userId && e.Id != excludeEntranceId);
        }
    }
}