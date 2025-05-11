using Microsoft.Identity.Client;

namespace EntranceManager.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Role { get; set; }

        public ICollection<Apartment> OwnedApartments { get; set; }

        public ICollection<ApartmentUser> ApartmentUsers { get; set; }

        public ICollection<Entrance> ManagedEntrances { get; set; }

        public ICollection<EntranceUser> EntranceUsers { get; set; }

    }
}
