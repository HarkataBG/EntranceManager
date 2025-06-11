namespace EntranceManager.Exceptions
{
    public class UserAlreadyAddedException : Exception
    {
        public UserAlreadyAddedException(string entity)
            : base($"User has already been added to this {entity} ") { }
    }
}
