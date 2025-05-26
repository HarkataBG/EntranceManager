using EntranceManager.Exceptions;
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
            var owner = await _userRepository.GetByIdAsync(apartment.OwnerUserId);
            if (owner == null)
                throw new OwnerNotFoundException(apartment.OwnerUserId);

            var entrance = await _entranceRepository.GetByIdAsync(apartment.EntranceId);
            if (entrance == null)
                throw new EntranceNotFoundException(apartment.EntranceId);

            await _apartmentRepository.AddAsync(apartment);
        }

        public async Task UpdateApartmentAsync(Apartment apartment)
        {
            var owner = await _userRepository.GetByIdAsync(apartment.OwnerUserId);
            if (owner == null)
                throw new OwnerNotFoundException(apartment.OwnerUserId);

            var entrance = await _entranceRepository.GetByIdAsync(apartment.EntranceId);
            if (entrance == null)
                throw new EntranceNotFoundException(apartment.EntranceId);

            await _apartmentRepository.UpdateAsync(apartment);
        }

        public async Task DeleteApartmentAsync(int id)
        {
            await _apartmentRepository.DeleteAsync(id);
        }

        public async Task AddUserToApartmentAsync(int apartmentId, int userId)
        {
            var apartment = await _apartmentRepository.GetByIdAsync(apartmentId);
            if (apartment == null)
                throw new ApartmentNotFoundException(apartmentId);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            bool alreadyAdded = apartment.ApartmentUsers
                .Any(au => au.UserId == userId);
            if (alreadyAdded)
                throw new InvalidOperationException("User is already added to this apartment.");

            var apartmentUser = new ApartmentUser
            {
                ApartmentId = apartmentId,
                UserId = userId
            };

            await _apartmentRepository.AddUserToApartmentAsync(apartmentUser);
        }
    }
}