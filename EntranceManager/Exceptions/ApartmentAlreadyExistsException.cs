namespace EntranceManager.Exceptions
{
    public class ApartmentAlreadyExistsException : Exception
    {
        public ApartmentAlreadyExistsException(int apartmentNumber, int entranceId)
            : base($"Apartment with number {apartmentNumber} already exists in entrance with ID {entranceId}") { }
    }
}
