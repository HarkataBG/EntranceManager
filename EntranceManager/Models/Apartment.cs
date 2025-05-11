namespace EntranceManager.Models
{
    public class Apartment
    {
        public int Id { get; set; }
        public int Floor { get; set; }
        public int Number { get; set; }
        public int OwnerUserId { get; set; }
        public User OwnerUser { get; set; }
        public int NumberOfLivingPeople {  get; set; }
        public int EntranceId { get; set; }
        public Entrance Entrance { get; set; }
        public ICollection<ApartmentFee> ApartmentFees { get; set; }

        public ICollection<ApartmentUser> ApartmentUsers { get; set; }
    }
}
