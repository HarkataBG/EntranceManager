using EntranceManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntranceManager.Repositories
{
    public interface IApartmentRepository
    {
        Task<IEnumerable<Apartment>> GetAllAsync();

        Task<Apartment> GetByIdAsync(int id);

        Task AddAsync(Apartment apartment);

        Task UpdateAsync(Apartment apartment);

        Task DeleteAsync(int id);

        Task<IEnumerable<Apartment>> GetApartmentsByOwnerAsync(int ownerUserId);

        Task<Apartment> GetApartmentWithFeesAsync(int id);
    }
}