using EntranceManager.Models.Mappers;

namespace EntranceManager.Services.Contracts
{
    public interface IPaymentsService
    {
        public Task ProcessPaymentAsync(PaymentDto dto);
    }
}
