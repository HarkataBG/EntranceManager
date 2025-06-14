using AspNetCoreDemo.Helpers;
using EntranceManager.Exceptions;
using EntranceManager.Exceptions.EntranceManager.Exceptions;
using EntranceManager.Helpers;
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
        private readonly IApartmentRepository _apartmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ModelMapper _mapper;

        public FeesService(IFeeRepository feeRepository, IEntranceRepository entranceRepository, IApartmentRepository apartmentRepository,
            IUserRepository userRepository, ModelMapper mapper)
        {
            _feeRepository = feeRepository;
            _entranceRepository = entranceRepository;
            _apartmentRepository = apartmentRepository;
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
            var entrance = await _entranceRepository.GetEntranceByIdAsync(dto.EntranceId, false)
                           ?? throw new EntranceNotFoundException(dto.EntranceId);

            var existingFee = await _feeRepository.GetByNameAsync(dto.Name);
            if (existingFee != null)
                throw new FeeAlreadyExistsException(dto.Name);

            var fee = _mapper.Map(dto);
            await _feeRepository.AddAsync(fee);

            var apartments = await _apartmentRepository.GetByEntranceIdAsync(entrance.Id);
            var apartmentFees = GetApartmentFees(fee, dto.FeeDetails, entrance, apartments);

            await _feeRepository.AddRangeAsync(apartmentFees);
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

        private List<ApartmentFee> GetApartmentFees(Fee fee, FeeDetails details, Entrance entrance, List<Apartment> apartments)
        {
            int totalApartments = apartments.Count;
            int totalResidents = apartments
                .Sum(a => CalculateHelper.CalculateResidents(a.ApartmentUsers.Count, a.NumberOfChildren, entrance.CountChildrenAsResidents));

            var calculatedFees = new Dictionary<int, decimal>();
            var apartmentFees = new List<ApartmentFee>();

            foreach (var apartment in apartments)
            {
                int apartmentResidents = CalculateHelper.CalculateResidents(
                    apartment.ApartmentUsers.Count,
                    apartment.NumberOfChildren,
                    entrance.CountChildrenAsResidents);

                decimal amount = CalculateHelper.CalculateFeeForApartment(
                    fee.Amount,
                    totalApartments,
                    totalResidents,
                    apartmentResidents,
                    apartment.NumberOfChildren,
                    apartment.NumberOfPets,
                    details);

                calculatedFees[apartment.Id] = amount;

                var apartmentFee = new ApartmentFee
                {
                    ApartmentId = apartment.Id,
                    FeeId = fee.Id,
                    AmountForApartment = amount
                };

                apartmentFees.Add(apartmentFee);
            }

            if (details.Normalize)
            {
                calculatedFees = CalculateHelper.NormalizeFees(calculatedFees, fee.Amount);

                foreach (var apartmentFee in apartmentFees)
                {
                    apartmentFee.AmountForApartment = calculatedFees[apartmentFee.ApartmentId];
                }
            }

            return apartmentFees;
        }

    }
}
