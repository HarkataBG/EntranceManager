using EntranceManager.Exceptions;
using EntranceManager.Models;
using EntranceManager.Models.Mappers;
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

        public async Task<IEnumerable<ApartmentResponseDto>> GetAllApartmentsDetailsAsync()
        {
            var apartments = await _apartmentRepository.GetAllWithDetailsAsync();

            var result = apartments.Select(a => new ApartmentResponseDto
            {
                Id = a.Id,
                Floor = a.Floor,
                Number = a.Number,
                NumberOfLivingPeople = a.ApartmentUsers?.Count ?? 0,
                Owner = new OwnerDto
                {
                    Id = a.OwnerUser.Id,
                    Username = a.OwnerUser.Username
                },
                Entrance = new EntranceDto
                {
                    Id = a.Entrance.Id,
                    City = a.Entrance.City,
                    Address = a.Entrance.Address,
                    EntranceSymbol = a.Entrance.EntranceSymbol
                },
                Residents = a.ApartmentUsers
                    .Select(au => new ResidentDto
                    {
                        Id = au.User.Id,
                        Username = au.User.Username
                    })
                    .ToList()
            }).ToList();

            return result;
        }

        public async Task<ApartmentResponseDto?> GetApartmentDetailsByIdAsync(int id)
        {
            var apartment = await _apartmentRepository.GetByIdAsync(id);

            return new ApartmentResponseDto
            {
                Id = apartment.Id,
                Floor = apartment.Floor,
                Number = apartment.Number,
                NumberOfLivingPeople = apartment.ApartmentUsers?.Count ?? 0,
                Owner = new OwnerDto
                {
                    Id = apartment.OwnerUser.Id,
                    Username = apartment.OwnerUser.Username
                },
                Entrance = new EntranceDto
                {
                    Id = apartment.Entrance.Id,
                    City = apartment.Entrance.City,
                    Address = apartment.Entrance.Address,
                    EntranceSymbol = apartment.Entrance.EntranceSymbol
                },
                Residents = apartment.ApartmentUsers
            .Select(au => new ResidentDto
            {
                Id = au.User.Id,
                Username = au.User.Username
            })
            .ToList()
            };
        }       

        public async Task<Apartment?> GetApartmentByIdAsync(int id)
        {
            return await _apartmentRepository.GetByIdAsync(id);
        }

        public async Task AddApartmentAsync(Apartment apartment)
        {
            var existingApartment = await _apartmentRepository.GetApartmentByNumber(apartment.Number, apartment.EntranceId);
            if (existingApartment != null)
                throw new ApartmentAlreadyExistsException(apartment.Number, apartment.EntranceId);

            var owner = await _userRepository.GetByIdAsync(apartment.OwnerUserId);
            if (owner == null)
                throw new OwnerNotFoundException(apartment.OwnerUserId);

            var entrance = await _entranceRepository.GetByIdAsync(apartment.EntranceId);
            if (entrance == null)
                throw new EntranceNotFoundException(apartment.EntranceId);

            await _apartmentRepository.AddAsync(apartment);

            var apartmentUser = new ApartmentUser
            {
                ApartmentId = apartment.Id,
                UserId = apartment.OwnerUserId
            };

            await _apartmentRepository.AddUserToApartmentAsync(apartmentUser);
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