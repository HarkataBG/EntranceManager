﻿using EntranceManager.Models;
using EntranceManager.Data;
using Microsoft.EntityFrameworkCore;
using EntranceManager.Models.Mappers;
using EntranceManager.Exceptions;
using AspNetCoreDemo.Helpers;

namespace EntranceManager.Repositories
{
    public class ApartmentRepository : IApartmentRepository
    {
        private readonly ApplicationContext _dbContext;

        public ApartmentRepository(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Apartment>> GetAllAsync()
        {
            return await _dbContext.Apartments.ToListAsync();
        }

        public async Task<List<Apartment>> GetByEntranceIdAsync(int entranceId)
        {
            return await _dbContext.Apartments
                .Where(a => a.EntranceId == entranceId)
                  .Include(a => a.ApartmentUsers)
                .ToListAsync();
        }

        public async Task<List<Apartment>> GetAllWithDetailsAsync(User currentUser)
        {
            IQueryable<Apartment> query = _dbContext.Apartments
                  .Include(a => a.ApartmentFees)
                      .ThenInclude(af => af.Fee)
                  .Include(a => a.ApartmentUsers)
                      .ThenInclude(au => au.User)
                  .Include(a => a.OwnerUser)
                  .Include(a => a.Entrance);

            return currentUser.Role switch
            {
                nameof(UserRole.Administrator) => await query.ToListAsync(),

                nameof(UserRole.EntranceManager) =>
                    await query
                        .Where(a => currentUser.ManagedEntrances
                            .Select(e => e.Id)
                            .Contains(a.EntranceId))
                        .ToListAsync(),

                _ =>
                    await query
                        .Where(a =>
                            a.OwnerUserId == currentUser.Id ||
                            a.ApartmentUsers.Any(au => au.UserId == currentUser.Id))
                        .ToListAsync()
            };
        }

        public async Task<Apartment> GetByIdAsync(int id)
        {
            return await _dbContext.Apartments
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Apartment> GetWithDetailsByIdAsync(int id)
        {
            return await _dbContext.Apartments
                .Include(a => a.ApartmentFees)
                    .ThenInclude(af => af.Fee)
                .Include(a => a.ApartmentUsers)
                    .ThenInclude(au => au.User)
                .Include(a => a.OwnerUser)
                .Include(a => a.Entrance)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Apartment> GetByIdWithApartmentUsers(int id)
        {
            return await _dbContext.Apartments
                  .Include(a => a.ApartmentUsers)
                      .ThenInclude(au => au.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Apartment> GetApartmentByNumber(int apartmentNumber, int entranceId)
        {
            return await _dbContext.Apartments
                .Where(a => a.EntranceId == entranceId)
                .FirstOrDefaultAsync(a => a.Number == apartmentNumber);
    }

        public async Task AddAsync(Apartment apartment)
        {
            await _dbContext.Apartments.AddAsync(apartment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Apartment apartment)
        {
            _dbContext.Apartments.Update(apartment); 
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var apartment = await _dbContext.Apartments.FindAsync(id);
            if (apartment != null)
            {
                _dbContext.Apartments.Remove(apartment);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Apartment>> GetApartmentsByOwnerAsync(int ownerUserId)
        {
            return await _dbContext.Apartments
                .Where(a => a.OwnerUserId == ownerUserId)
                .ToListAsync();
        }

        public async Task<Apartment> GetApartmentWithFeesAsync(int id)
        {
            return await _dbContext.Apartments
                .Include(a => a.ApartmentFees)
                .ThenInclude(af => af.Fee)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddUserToApartmentAsync(ApartmentUser apartmentUser)
        {
            await _dbContext.ApartmentUsers.AddAsync(apartmentUser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ApartmentUser> GetApartmentUserAsync(int userId, int apartmentId)
        {
            return await _dbContext.ApartmentUsers
                 .FirstOrDefaultAsync(au => au.UserId == userId && au.ApartmentId == apartmentId);
        }

        public async Task RemoveUserFromApartmentAsync(ApartmentUser apartmentUser)
        {            
            _dbContext.ApartmentUsers.Remove(apartmentUser);
            await _dbContext.SaveChangesAsync();
        }
    }
}