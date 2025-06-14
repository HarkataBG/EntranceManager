using EntranceManager.Models;

namespace EntranceManager.Repositories.Contracts
{
    public interface IPaymentRepository
    {
        Task<ApartmentFee> GetApartmentFeeWithPaymentsAsync(int apartmentId, int feeId);
        Task UpdateApartmentFeeAsync(ApartmentFee apartmentFee);
        Task AddPaymentAsync(Payment payment);
    }
}
