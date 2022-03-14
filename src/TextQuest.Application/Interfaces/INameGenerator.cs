namespace TextQuest.Application.Interfaces
{
    public interface INameGenerator
    {
        string GetName(NameGenerationParams generationParam);
    }
}
