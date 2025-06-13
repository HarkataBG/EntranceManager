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

        public async Task<List<Entrance>> GetAllEntrancesAsync(User currentUser, bool withDetails)
        {
            IQueryable<Entrance> query;
            if (withDetails)
            {
                query = _dbContext.Entrances
                               .Include(e => e.EntranceUsers)
                                   .ThenInclude(eu => eu.User)
                               .Include(e => e.Apartments);
            }
            else
            {
                query = _dbContext.Entrances;
            }

            return currentUser.Role switch
            {
                nameof(UserRole.Administrator) => await query.ToListAsync(),

                nameof(UserRole.EntranceManager) =>
                    await query
                        .Where(e => currentUser.ManagedEntrances
                            .Select(me => me.Id)
                            .Contains(e.Id))
                        .ToListAsync(),

                _ =>
                    await query
                        .Where(e => e.EntranceUsers
                            .Any(eu => eu.UserId == currentUser.Id))
                        .ToListAsync()
            };
        }

        public async Task<Entrance> GetEntranceByIdAsync(int id, bool withDetails)
        {
            IQueryable<Entrance> query;
            if (withDetails)
            {
                query = _dbContext.Entrances
                                .Include(a => a.EntranceUsers)
                                   .ThenInclude(au => au.User)
                               .Include(a => a.Apartments);
                              
            }
            else
            {
                query = _dbContext.Entrances;
            }
            return await query.FirstOrDefaultAsync(e => e.Id == id);

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