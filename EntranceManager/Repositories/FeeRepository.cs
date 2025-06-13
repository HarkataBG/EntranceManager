using EntranceManager.Data;
using EntranceManager.Models;
using EntranceManager.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using System;

namespace EntranceManager.Repositories
{
    public class FeeRepository : IFeeRepository
    {
        private readonly ApplicationContext _dbContext;

        public FeeRepository(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Fee>> GetAllFeeDetailsAsync()
        {
            return await _dbContext.Fees
                .Include(e => e.Entrance)
                .Include(af => af.ApartmentFees)
                    .ThenInclude(a => a.Apartment)
                .ToListAsync();
        }

        public async Task<Fee> GetFeeDetailsByIdAsync(int id)
        {
            return await _dbContext.Fees
             .Include(e => e.Entrance)
             .Include(af => af.ApartmentFees)
                 .ThenInclude(a => a.Apartment)
             .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Fee> GetByIdAsync(int id)
        {
            return await _dbContext.Fees
             .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task AddAsync(Fee fee)
        {
            await _dbContext.Fees.AddAsync(fee);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Fee fee)
        {
            _dbContext.Fees.Update(fee);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int feeId)
        {
            var fee = await _dbContext.Fees.FindAsync(feeId);
            if (fee != null)
            {
                _dbContext.Fees.Remove(fee);
                await _dbContext.SaveChangesAsync();
            }
        }
    }

}
