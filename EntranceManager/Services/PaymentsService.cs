using Azure.Core;
using EntranceManager.Exceptions;
using EntranceManager.Models;
using EntranceManager.Models.Mappers;
using EntranceManager.Repositories.Contracts;
using EntranceManager.Services.Contracts;

namespace EntranceManager.Services
{
    public class PaymentsService : IPaymentsService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentsService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task ProcessPaymentAsync(PaymentDto dto)
        {
            var apartmentFee = await _paymentRepository.GetApartmentFeeWithPaymentsAsync(dto.ApartmentId, dto.FeeId);
            if (apartmentFee == null)
                throw new FeeNotFoundException(dto.FeeId);

            if (dto.Amount <= 0 || dto.Amount > apartmentFee.AmountForApartment)
                throw new InvalidOperationException("Sum is not valid");

            var existingTotalPaid = apartmentFee.Payments.Sum(p => p.AmountPaid);
            var totalPaid = existingTotalPaid + dto.Amount;

            if (totalPaid > apartmentFee.AmountForApartment)
                throw new InvalidOperationException("This payment would overpay the fee.");

            var payment = new Payment
            {
                ApartmentFeeId = apartmentFee.Id,
                AmountPaid = dto.Amount,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = dto.PaymentMethod
            };

            await _paymentRepository.AddPaymentAsync(payment);

            apartmentFee.AmountAlreadyPaid = totalPaid;

            if (totalPaid >= apartmentFee.AmountForApartment)
            {
                apartmentFee.IsPaid = true;
                apartmentFee.PaymentDate = DateTime.UtcNow;
            }

            await _paymentRepository.UpdateApartmentFeeAsync(apartmentFee);
        }
    }
}
