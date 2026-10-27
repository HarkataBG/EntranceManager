using EntranceManager.Exceptions;
using EntranceManager.Models;
using EntranceManager.Models.AI;
using EntranceManager.Repositories.Contracts;
using EntranceManager.Services.Contracts;

namespace EntranceManager.Services
{
    public class EntranceFinancialService : IEntranceFinancialService
    {
        private readonly IFeeRepository _feeRepository;
        private readonly IUserRepository _userRepository;

        public EntranceFinancialService(
            IFeeRepository feeRepository,
            IUserRepository userRepository)
        {
            _feeRepository = feeRepository;
            _userRepository = userRepository;
        }

        public async Task<IReadOnlyList<FinancialSummaryDto>> GetAccessibleFinancialSummaryAsync(
            string username,
            CancellationToken cancellationToken = default)
        {
            var currentUser = await _userRepository.GetByUsernameAsync(username)
                ?? throw new UserNotFoundException(username);

            var fees = await _feeRepository.GetAllFinancialDetailsAsync(currentUser);

            var summaries = fees
                .Where(fee => fee.Entrance != null)
                .GroupBy(fee => fee.EntranceId)
                .Select(entranceFees =>
                {
                    var firstFee = entranceFees.First();

                    var feeSummaries = entranceFees
                        .SelectMany(fee => fee.ApartmentFees ?? new List<ApartmentFee>())
                        .Where(apartmentFee => apartmentFee.Apartment != null)
                        .Select(apartmentFee =>
                        {
                            var outstandingAmount =
                                apartmentFee.AmountForApartment -
                                apartmentFee.AmountAlreadyPaid;

                            return new FinancialFeeSummaryDto
                            {
                                ApartmentId = apartmentFee.ApartmentId,
                                ApartmentNumber = apartmentFee.Apartment.Number,
                                FeeId = apartmentFee.FeeId,
                                FeeName = apartmentFee.Fee.Name,
                                AmountForApartment = apartmentFee.AmountForApartment,
                                AmountAlreadyPaid = apartmentFee.AmountAlreadyPaid,
                                OutstandingAmount = outstandingAmount,
                                IsPaid = apartmentFee.IsPaid,
                                PaymentDate = apartmentFee.PaymentDate
                            };
                        })
                        .ToList();

                    return new FinancialSummaryDto
                    {
                        EntranceId = firstFee.EntranceId,
                        EntranceName = firstFee.Entrance.EntranceName,
                        Address = firstFee.Entrance.Address,
                        TotalExpectedAmount = feeSummaries
                            .Sum(item => item.AmountForApartment),
                        TotalPaidAmount = feeSummaries
                            .Sum(item => item.AmountAlreadyPaid),
                        TotalOutstandingAmount = feeSummaries
                            .Sum(item => item.OutstandingAmount),
                        PaidCount = feeSummaries.Count(item =>
                            item.AmountAlreadyPaid >= item.AmountForApartment),
                        PartiallyPaidCount = feeSummaries.Count(item =>
                            item.AmountAlreadyPaid > 0 &&
                            item.AmountAlreadyPaid < item.AmountForApartment),
                        UnpaidCount = feeSummaries.Count(item =>
                            item.AmountAlreadyPaid == 0),
                        Fees = feeSummaries
                    };
                })
                .ToList();

            return summaries;
        }
    }
}