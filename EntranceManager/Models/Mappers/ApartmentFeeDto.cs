namespace EntranceManager.Models.Mappers
{
    public class ApartmentFeeDto
    {
        public int Id { get; set; }

        public int ApartmentId { get; set; }

        public bool IsPaid { get; set; } = false;
        public DateTime? PaymentDate { get; set; }
    }
}
