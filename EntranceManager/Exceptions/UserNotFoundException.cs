namespace EntranceManager.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(int userId)
            : base($"User with ID {userId} was not found.") { }

        public UserNotFoundException(string username)
            : base($"User with username {username} was not found.") { }
    }
}
