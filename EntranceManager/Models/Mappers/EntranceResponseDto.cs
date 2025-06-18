namespace EntranceManager.Models.Mappers
{
    public class EntranceResponseDto
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public int PostCode { get; set; }
        public string EntranceName { get; set; }
        public bool CountChildrenAsResidents { get; set; }

        public ManagerDto Manager { get; set; }

        public List<ApartmentSummaryDto> Apartments { get; set; }

        public List<ResidentDto> Residents { get; set; }
    }
}
