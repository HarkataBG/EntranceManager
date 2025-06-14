namespace EntranceManager.Models
{
    public class ApartmentFee
    {
        public int Id { get; set; }

        public int ApartmentId { get; set; }
        public Apartment Apartment { get; set; }
        public int FeeId { get; set; }
        public Fee Fee { get; set; }
        public bool IsPaid { get; set; } = false;
        public DateTime? PaymentDate { get; set; }

        public decimal AmountForApartment { get; set; }
        public decimal AmountAlreadyPaid { get; set; }

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
