namespace TextQuest.Application.Exceptions
{
    public class ItemExchangeException : Exception
    {
        public ItemExchangeException(string message) : base($"Cant exchange items: {message}")
        {
        }
    }
}
