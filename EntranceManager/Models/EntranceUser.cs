namespace EntranceManager.Models
{
    public class EntranceUser
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int EntranceId { get; set; }
        public Entrance Entrance { get; set; }
    }
}