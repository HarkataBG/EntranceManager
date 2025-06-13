using AspNetCoreDemo.Helpers;
using EntranceManager.Exceptions;
using EntranceManager.Models;
using EntranceManager.Models.Mappers;
using EntranceManager.Repositories;
using EntranceManager.Repositories.Contracts;
using EntranceManager.Services.Contracts;

namespace EntranceManager.Services
{
    public class FeesService : IFeesService
    {
        private readonly IFeeRepository _feeRepository;
        private readonly IEntranceRepository _entranceRepository;
        private readonly IUserRepository _userRepository;
        private readonly ModelMapper _mapper;

        public FeesService(IFeeRepository feeRepository, IEntranceRepository entranceRepository, IUserRepository userRepository, ModelMapper mapper)
        {
            _feeRepository = feeRepository;
            _entranceRepository = entranceRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FeeResponseDto>> GetAllFeeDetailsAsync(string username)
        {
            var currentUser = await _userRepository.GetByUsernameAsync(username)
               ?? throw new UserNotFoundException(username);

            var fees = await _feeRepository.GetAllFeeDetailsAsync(currentUser);
            return fees.Select(f => _mapper.Map(f));
        }

        public async Task<FeeResponseDto?> GetFeeDetailsByIdAsync(int id)
        {
            var fee = await _feeRepository.GetFeeDetailsByIdAsync(id);

            if (fee == null)
                throw new FeeNotFoundException(id);

            return _mapper.Map(fee);
        }

        public async Task<Fee> GetByIdAsync(int id)
        {
            var fee = await _feeRepository.GetByIdAsync(id)
                      ?? throw new FeeNotFoundException(id);

            return fee;
        }

        public async Task CreateFeeAsync(FeeDto dto)
        {
            _ = await _entranceRepository.GetEntranceByIdAsync(dto.EntranceId, false)
                           ?? throw new EntranceNotFoundException(dto.EntranceId);

            var fee = _mapper.Map(dto);
            await _feeRepository.AddAsync(fee);
        }

        public async Task UpdateFeeAsync(int id, FeeDto dto)
        {
            var fee = await _feeRepository.GetByIdAsync(id)
                      ?? throw new FeeNotFoundException(id);

            _ = await _entranceRepository.GetEntranceByIdAsync(dto.EntranceId, false)
                          ?? throw new EntranceNotFoundException(dto.EntranceId);

            _mapper.Map(dto, fee); 
            await _feeRepository.UpdateAsync(fee);
        }

        public async Task DeleteFeeAsync(int feeId)
        {
            await _feeRepository.DeleteAsync(feeId);
        }
    }
}
