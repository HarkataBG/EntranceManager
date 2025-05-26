namespace EntranceManager.Exceptions
{
    public class ApartmentNotFoundException : Exception
    {
        public ApartmentNotFoundException(int apartmentId)
            : base($"Apartment with ID {apartmentId} was not found.") { }
    }
}
