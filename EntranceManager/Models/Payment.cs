namespace EntranceManager.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int ApartmentFeeId { get; set; }
        public ApartmentFee ApartmentFee { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
    }
}