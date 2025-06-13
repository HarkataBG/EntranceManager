using EntranceManager.Models;

public class Entrance
{
    public int Id { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public int PostCode { get; set; }
    public string EntranceName { get; set; }

    public int? ManagerUserId { get; set; } 
    public User ManagerUser { get; set; }

    public bool CountChildrenAsResidents { get; set; }
    public ICollection<Apartment> Apartments { get; set; }
    public ICollection<EntranceUser> EntranceUsers { get; set; }
}