using System.Collections.Generic;

namespace EntranceManager.Models.AI
{
    public class FinancialSummaryDto
    {
        public int EntranceId { get; set; }
        public string EntranceName { get; set; }
        public string Address { get; set; }

        public decimal TotalExpectedAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal TotalOutstandingAmount { get; set; }

        public int PaidCount { get; set; }
        public int PartiallyPaidCount { get; set; }
        public int UnpaidCount { get; set; }

        public List<FinancialFeeSummaryDto> Fees { get; set; } =
            new List<FinancialFeeSummaryDto>();
    }
}