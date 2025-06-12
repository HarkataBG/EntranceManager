namespace EntranceManager.Exceptions
{
    public class EntranceAlreadyExistsException : Exception
    {
        public EntranceAlreadyExistsException(string entranceName, string address)
            : base($"Entrance with name {entranceName} and address {address} already exists") { }
    }
}
