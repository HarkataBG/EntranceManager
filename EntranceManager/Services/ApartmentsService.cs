using EntranceManager.Models;
using EntranceManager.Repositories;

namespace EntranceManager.Services
{
    public class ApartmentService : IApartmentService
    {
        private readonly IApartmentRepository _apartmentRepository;
        private readonly IEntranceRepository _entranceRepository;
        private readonly IUserRepository _userRepository;

        public ApartmentService(IApartmentRepository apartmentRepository, IEntranceRepository entranceRepository, IUserRepository userRepository)
        {
            _apartmentRepository = apartmentRepository;
            _entranceRepository = entranceRepository;
            _userRepository = userRepository;
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
            var owner = await _userRepository.GetByIdAsync(apartment.Id);
            if (owner == null)
                throw new Exception();

            var entrance = await _entranceRepository.GetByIdAsync(apartment.EntranceId);
            if (entrance == null)
                throw new Exception();

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