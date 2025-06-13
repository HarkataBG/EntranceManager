namespace EntranceManager.Models.Mappers
{
    public class FeeDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int EntranceId { get; set; }
    }
}
