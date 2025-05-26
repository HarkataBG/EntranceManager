namespace EntranceManager.Exceptions
{
    public class OwnerNotFoundException : Exception
    {
        public OwnerNotFoundException(int userId)
            : base($"Owner with ID {userId} was not found.") { }
    }
}
