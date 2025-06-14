namespace EntranceManager.Exceptions
{
    namespace EntranceManager.Exceptions
    {
        public class FeeAlreadyExistsException : Exception
        {
            public FeeAlreadyExistsException(string feeName)
                : base($"Fee with name '{feeName}' already exists.") { }
        }
    }

}
