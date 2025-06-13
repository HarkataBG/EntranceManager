namespace EntranceManager.Models.Mappers
{
    public class EntranceDto
    {
        public string City { get; set; }
        public string Address { get; set; }
        public string EntranceName { get; set; }
        public int PostCode { get; set; }
        public bool CountChildrenAsResidents {  get; set; }
    }
}
