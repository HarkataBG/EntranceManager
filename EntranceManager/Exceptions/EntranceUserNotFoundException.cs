namespace EntranceManager.Exceptions
{
    public class EntranceUserNotFoundException : Exception
    {
        public EntranceUserNotFoundException(int userId, int entranceId)
            : base($"User with {userId} was not found as a resident for entrance {entranceId}.") { }
    }
}
