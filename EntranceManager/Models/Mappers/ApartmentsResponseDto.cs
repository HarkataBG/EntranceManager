namespace EntranceManager.Models.Mappers
{
    public class ApartmentResponseDto
    {
        public int Id { get; set; }
        public int Floor { get; set; }
        public int Number { get; set; }
        public int NumberOfLivingPeople { get; set; }

        public OwnerDto Owner { get; set; }
        public EntranceSummaryDto Entrance { get; set; }
        public List<ResidentDto> Residents { get; set; }

        public List<FeeSummaryDto> Fees { get; set; }

        public int NumberOfChildren { get; set; }
        public int NumberOfPets { get; set; }
    }
}
