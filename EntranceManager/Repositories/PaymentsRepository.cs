using EntranceManager.Data;
using EntranceManager.Models;
using EntranceManager.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationContext _dbContext;

    public PaymentRepository(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApartmentFee> GetApartmentFeeWithPaymentsAsync(int apartmentId, int feeId)
    {
        return await _dbContext.ApartmentFees
            .Include(af => af.Payments)
            .FirstOrDefaultAsync(af => af.ApartmentId == apartmentId && af.FeeId == feeId);
    }

    public async Task UpdateApartmentFeeAsync(ApartmentFee apartmentFee)
    {
        _dbContext.ApartmentFees.Update(apartmentFee);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddPaymentAsync(Payment payment)
    {
        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync();
    }
}