using AspNetCoreDemo.Helpers;
using EntranceManager.Exceptions;
using EntranceManager.Models;
using EntranceManager.Models.Mappers;
using EntranceManager.Repositories;
using EntranceManager.Services.Contracts;

namespace EntranceManager.Services
{
    public class EntranceService : IEntranceService
    {
        private readonly IEntranceRepository _entranceRepository;
        private readonly IUserRepository _userRepository;
        private readonly ModelMapper _mapper;

        public EntranceService(
            IEntranceRepository entranceRepository,
            IUserRepository userRepository,
            ModelMapper mapper)
        {
            _entranceRepository = entranceRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EntranceResponseDto>> GetAllEntrancesDetailsAsync(string username)
        {
            var currentUser = await _userRepository.GetByUsernameAsync(username)
               ?? throw new UserNotFoundException(username);

            var entrances = await _entranceRepository.GetAllEntrancesAsync(currentUser, true);
            return entrances.Select(e => _mapper.Map(e));
        }

        public async Task<EntranceResponseDto> GetEntranceDetailsByIdAsync(int id)
        {
            var entrance = await _entranceRepository.GetEntranceByIdAsync(id, true);
            
            if (entrance == null)
                throw new EntranceNotFoundException(id);

            return _mapper.Map(entrance);
        }

        public async Task<Entrance?> GetEntranceByIdAsync(int id)
        {
            var entrance = await _entranceRepository.GetEntranceByIdAsync(id, false);
            if (entrance == null)
                throw new EntranceNotFoundException(id);

            return entrance;
        }

        public async Task AddEntranceAsync(EntranceDto dto)
        {
            var existingEntrance = await _entranceRepository.GetEntranceByNameAndAdress(dto.EntranceName, dto.Address);
            if (existingEntrance != null)
                throw new EntranceAlreadyExistsException(dto.EntranceName, dto.Address);

            var entrance = _mapper.Map(dto);
            await _entranceRepository.AddAsync(entrance);
        }

        public async Task UpdateEntranceAsync(int id, EntranceDto dto)
        {
            var existingEntrance = await _entranceRepository.GetEntranceByNameAndAdress(dto.EntranceName, dto.Address);
            if (existingEntrance != null)
                throw new EntranceAlreadyExistsException(dto.EntranceName, dto.Address);

            var entrance = await _entranceRepository.GetEntranceByIdAsync(id, false);
            if (entrance == null)
                throw new EntranceNotFoundException(id);

            _mapper.Map(dto, entrance);
            await _entranceRepository.UpdateAsync(entrance);
        }

        public async Task DeleteEntranceAsync(int id)
        {
            var existing = await _entranceRepository.GetEntranceByIdAsync(id, false);
            if (existing == null)
                throw new EntranceNotFoundException(id);

            await _entranceRepository.DeleteAsync(id);
        }

        public async Task AddUserToEntranceAsync(int entranceId, int userId)
        {
            var entrance = await _entranceRepository.GetEntranceByIdAsync(entranceId, true)
                          ?? throw new EntranceNotFoundException(entranceId);
            var user = await _userRepository.GetByIdAsync(userId)
                       ?? throw new UserNotFoundException(userId);

            var exists = entrance.EntranceUsers.Any(eu => eu.UserId == userId);
            if (exists)
                throw new UserAlreadyAddedException("entrance");

            await _entranceRepository.AddUserToEntranceAsync(new EntranceUser
            {
                EntranceId = entranceId,
                UserId = userId
            });
        }

        public async Task RemoveUserFromEntrance(int entranceId, int userId)
        {
            var entrance = await _entranceRepository.GetEntranceByIdAsync(entranceId, false);
            if (entrance == null)
                throw new EntranceNotFoundException(entranceId);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFoundException(userId);

            var entranceUser = await _entranceRepository.GetEntranceUserAsync(userId, entranceId);
            if (entranceUser == null)
                throw new EntranceUserNotFoundException(userId, entranceId);

            await _entranceRepository.RemoveUserFromEntranceAsync(entranceUser);
        }
    }
}