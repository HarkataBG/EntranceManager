namespace EntranceManager.Models.Mappers
{
    public class FeeSummaryDto
    {
        public int Id { get; set; } 
        public int FeeId { get; set; }
        public string Name { get; set; }  
        public string Description { get; set; }  
        public decimal Amount { get; set; }  
        public bool IsPaid { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal AmountForApartment { get; set; }
        public decimal AmountAlreadyPaid { get; set; }
    }
}
