namespace EntranceManager.Models.Mappers
{
    public class PaymentDto
    {
        public int ApartmentId { get; set; }
        public int FeeId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
    }
}
