using EntranceManager.Data;
using EntranceManager.Models;
using Microsoft.EntityFrameworkCore;

namespace EntranceManager.Repositories
{
    public class EntranceRepository : IEntranceRepository
    {
        private readonly ApplicationContext _context;

        public EntranceRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Entrance>> GetAllAsync()
        {
            return await _context.Entrances.ToListAsync();
        }

        public async Task<Entrance?> GetByIdAsync(int id)
        {
            return await _context.Entrances
                .Include(e => e.Apartments) 
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AddAsync(Entrance entrance)
        {
            _context.Entrances.Add(entrance);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Entrance entrance)
        {
            _context.Entrances.Update(entrance);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entrance = await _context.Entrances.FindAsync(id);
            if (entrance != null)
            {
                _context.Entrances.Remove(entrance);
                await _context.SaveChangesAsync();
            }
        }
    }
}