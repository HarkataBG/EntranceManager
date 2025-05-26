namespace EntranceManager.Exceptions
{
    public class EntranceNotFoundException : Exception
    {
        public EntranceNotFoundException(int entranceId)
            : base($"Entrance with ID {entranceId} was not found.") { }
    }
}
