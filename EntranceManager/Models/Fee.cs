namespace EntranceManager.Models
{
    public class Fee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int EntranceId { get; set; }
        public Entrance Entrance { get; set; }
        public ICollection<ApartmentFee> ApartmentFees { get; set; }
    }
}
