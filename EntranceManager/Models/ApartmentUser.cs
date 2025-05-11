namespace EntranceManager.Models
{
    public class ApartmentUser
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int ApartmentId { get; set; }
        public Apartment Apartment { get; set; }
    }
}