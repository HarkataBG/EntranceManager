namespace EntranceManager.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // "Administrator", "Owner", "User"
        public ICollection<Apartment> Apartments { get; set; }
    }
}
