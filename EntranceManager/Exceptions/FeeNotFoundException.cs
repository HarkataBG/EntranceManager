namespace EntranceManager.Exceptions
{
    public class FeeNotFoundException : Exception
    {
        public FeeNotFoundException(int feeId)
            : base($"Fee with ID {feeId} was not found.") { }
    }
}
