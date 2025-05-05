using EntranceManager.Models;
using EntranceManager.Repositories;

namespace EntranceManager.Services
{
    public class ApartmentService : IApartmentService
    {
        private readonly IApartmentRepository _apartmentRepository;

        public ApartmentService(IApartmentRepository apartmentRepository)
        {
            _apartmentRepository = apartmentRepository;
        }

        public async Task<IEnumerable<Apartment>> GetAllApartmentsAsync()
        {
            return await _apartmentRepository.GetAllAsync();
        }

        public async Task<Apartment?> GetApartmentByIdAsync(int id)
        {
            return await _apartmentRepository.GetByIdAsync(id);
        }

        public async Task AddApartmentAsync(Apartment apartment)
        {
            await _apartmentRepository.AddAsync(apartment);
        }

        public async Task UpdateApartmentAsync(Apartment apartment)
        {
            await _apartmentRepository.UpdateAsync(apartment);
        }

        public async Task DeleteApartmentAsync(int id)
        {
            await _apartmentRepository.DeleteAsync(id);
        }
    }
}