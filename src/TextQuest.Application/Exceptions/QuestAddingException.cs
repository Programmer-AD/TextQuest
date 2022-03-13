namespace TextQuest.Application.Exceptions
{
    public class QuestAddingException : Exception
    {
        public QuestAddingException(string message) : base($"Cant add quest: {message}")
        {
        }
    }
}
