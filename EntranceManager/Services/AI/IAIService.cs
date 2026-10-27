using EntranceManager.Models.AI;

namespace EntranceManager.Services.AI
{
    public interface IAIService
    {
        Task<string> GenerateFinancialSummaryAsync(
            FinancialSummaryDto financialSummary,
            CancellationToken cancellationToken = default);
    }
}