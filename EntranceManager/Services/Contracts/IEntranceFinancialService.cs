using EntranceManager.Models.AI;

namespace EntranceManager.Services.Contracts
{
    public interface IEntranceFinancialService
    {
        Task<IReadOnlyList<FinancialSummaryDto>> GetAccessibleFinancialSummaryAsync(
            string username,
            CancellationToken cancellationToken = default);
    }
}