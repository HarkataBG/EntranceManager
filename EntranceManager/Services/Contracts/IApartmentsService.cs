using EntranceManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntranceManager.Services
{
    public interface IApartmentService
    {
        Task<IEnumerable<Apartment>> GetAllApartmentsAsync();

        Task<Apartment?> GetApartmentByIdAsync(int id);

        Task AddApartmentAsync(Apartment apartment);

        Task UpdateApartmentAsync(Apartment apartment);

        Task DeleteApartmentAsync(int id);
    }
}