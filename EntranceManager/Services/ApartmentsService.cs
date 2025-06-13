using AspNetCoreDemo.Helpers;
using EntranceManager.Exceptions;
using EntranceManager.Models;
using EntranceManager.Models.Mappers;
using EntranceManager.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace EntranceManager.Services
{
    public class ApartmentService : IApartmentService
    {
        private readonly IApartmentRepository _apartmentRepository;
        private readonly IEntranceRepository _entranceRepository;
        private readonly IUserRepository _userRepository;
        private readonly ModelMapper _mapper;

        public ApartmentService(IApartmentRepository apartmentRepository, IEntranceRepository entranceRepository, IUserRepository userRepository, ModelMapper mapper)
        {
            _apartmentRepository = apartmentRepository;
            _entranceRepository = entranceRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Apartment>> GetAllApartmentsAsync()
        {
            return await _apartmentRepository.GetAllAsync();
        }

        public async Task<IEnumerable<ApartmentResponseDto>> GetAllApartmentsDetailsAsync()
        {
            var apartments = await _apartmentRepository.GetAllWithDetailsAsync();

            return apartments.Select(a => _mapper.Map(a));
        }

        public async Task<ApartmentResponseDto?> GetApartmentDetailsByIdAsync(int id)
        {
            var apartment = await _apartmentRepository.GetWithDetailsByIdAsync(id);

            if (apartment == null)
                throw new ApartmentNotFoundException(id);

            return _mapper.Map(apartment);
        }

        public async Task<Apartment?> GetApartmentByIdAsync(int id)
        {
            var apartment = await _apartmentRepository.GetByIdAsync(id);
            if (apartment == null)
                throw new ApartmentNotFoundException(id);

            return await _apartmentRepository.GetByIdAsync(id);
        }

        public async Task AddApartmentAsync(ApartmentDto dto)
        {
            var apartment = _mapper.Map(dto);

            var existingApartment = await _apartmentRepository.GetApartmentByNumber(apartment.Number, apartment.EntranceId);
            if (existingApartment != null)
                throw new ApartmentAlreadyExistsException(apartment.Number, apartment.EntranceId);

            var owner = await _userRepository.GetByIdAsync(apartment.OwnerUserId);
            if (owner == null)
                throw new OwnerNotFoundException(apartment.OwnerUserId);

            var entranceUser = await _entranceRepository.GetEntranceUserAsync(dto.OwnerUserId, dto.EntranceId);
            if (entranceUser == null)
                throw new UnauthorizedAccessException("Owner must be a resident of the entrance.");

            var entrance = await _entranceRepository.GetByIdAsync(apartment.EntranceId);
            if (entrance == null)
                throw new EntranceNotFoundException(apartment.EntranceId);

            await _apartmentRepository.AddAsync(apartment);
        }

        public async Task UpdateApartmentAsync(int apartmentId, ApartmentDto dto)
        {
            var owner = await _userRepository.GetByIdAsync(dto.OwnerUserId);
            if (owner == null)
                throw new OwnerNotFoundException(dto.OwnerUserId);

            var entranceUser = await _entranceRepository.GetEntranceUserAsync(dto.OwnerUserId, dto.EntranceId);
            if (entranceUser == null)
                throw new UnauthorizedAccessException("Owner must be a resident of the entrance.");

            var entrance = await _entranceRepository.GetByIdAsync(dto.EntranceId);
            if (entrance == null)
                throw new EntranceNotFoundException(dto.EntranceId);

            var apartment = await _apartmentRepository.GetByIdAsync(apartmentId);
            if (apartment == null)
                throw new ApartmentNotFoundException(apartmentId);

            _mapper.Map(dto, apartment);

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
                throw new UserAlreadyAddedException("apartment");

            var entranceUser = await _entranceRepository.GetEntranceUserAsync(apartment.OwnerUserId, apartment.EntranceId);
            if (entranceUser == null)
                throw new UnauthorizedAccessException("Owner must be a resident of the entrance.");

            var apartmentUser = new ApartmentUser
            {
                ApartmentId = apartmentId,
                UserId = userId
            };

            await _apartmentRepository.AddUserToApartmentAsync(apartmentUser);
        }

        public async Task RemoveUserFromApartment(int apartmentId, int userId)
        {
            var apartment = await _apartmentRepository.GetByIdAsync(apartmentId);
            if (apartment == null)
                throw new ApartmentNotFoundException(apartmentId);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            var apartmentUser = await _apartmentRepository.GetApartmentUserAsync(userId, apartmentId);
            if (apartmentUser == null)
                throw new ApartmentUserNotFoundException(userId, apartmentId);

            await _apartmentRepository.RemoveUserFromApartmentAsync(apartmentUser);
        }
    }
}