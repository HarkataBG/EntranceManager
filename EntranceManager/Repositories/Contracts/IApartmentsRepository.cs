using EntranceManager.Models;
using EntranceManager.Models.Mappers;
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

        Task AddUserToApartmentAsync(ApartmentUser apartmentUser);

        Task<Apartment> GetApartmentByNumber(int number, int entranceId);

        Task<List<Apartment>> GetByEntranceIdAsync(int entranceId);

        Task<List<Apartment>> GetAllWithDetailsAsync(User currentUser);

        Task<Apartment> GetWithDetailsByIdAsync(int id);

        Task<Apartment> GetByIdWithApartmentUsers(int id);

        Task<ApartmentUser> GetApartmentUserAsync(int userId, int apartmentId);

        Task RemoveUserFromApartmentAsync(ApartmentUser apartmentUser);
    }
}