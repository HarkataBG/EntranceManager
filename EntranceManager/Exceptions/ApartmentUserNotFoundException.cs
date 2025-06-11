namespace EntranceManager.Exceptions
{
    public class ApartmentUserNotFoundException : Exception
    {
        public ApartmentUserNotFoundException(int userId, int apartmentId)
            : base($"User with {userId} was not found as a resident for apartment {apartmentId}.") { }
    }
}
