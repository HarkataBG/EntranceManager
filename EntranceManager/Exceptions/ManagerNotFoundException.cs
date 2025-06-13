namespace EntranceManager.Exceptions
{
    public class ManagerNotFoundException : Exception
    {
        public ManagerNotFoundException(int entranceId)
            : base($"Manager for entrance with ID {entranceId} was not found.") { }
    }
}
