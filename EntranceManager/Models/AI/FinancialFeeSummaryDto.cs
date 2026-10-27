namespace EntranceManager.Models.AI
{
    public class FinancialFeeSummaryDto
    {
        public int ApartmentId { get; set; }
        public int ApartmentNumber { get; set; }

        public int FeeId { get; set; }
        public string FeeName { get; set; }

        public decimal AmountForApartment { get; set; }
        public decimal AmountAlreadyPaid { get; set; }
        public decimal OutstandingAmount { get; set; }

        public bool IsPaid { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}