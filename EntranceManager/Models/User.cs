namespace EntranceManager.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Role { get; set; } // "Administrator", "Owner", "User"
        public ICollection<Apartment> Apartments { get; set; }
    }
}
