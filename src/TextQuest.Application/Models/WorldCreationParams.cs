namespace TextQuest.Application.Models
{
    public record struct WorldCreationParams(
        Range QuestCount,
        int MaxQuestsForCharacter,
        int MaxCharactersInLocation);
}
