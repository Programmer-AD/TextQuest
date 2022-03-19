namespace TextQuest.Application.Models
{
    public record struct WorldCreationParams(
        Range QuestCount,
        Range ItemTypeCount,
        int MaxItemCount,
        int MaxQuestsForCharacter,
        int MaxCharactersInLocation);
}
