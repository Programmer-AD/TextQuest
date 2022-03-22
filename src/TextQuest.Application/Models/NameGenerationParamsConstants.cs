namespace TextQuest.Application.Models
{
    internal static class NameGenerationParamsConstants
    {
        public static readonly NameGenerationParams ItemName = new(1..3, 2..4);
        public static readonly NameGenerationParams MonsterName = new(2..3, 2..4);
        public static readonly NameGenerationParams CharacterName = new(2..2, 2..4);
        public static readonly NameGenerationParams LocationName = new(1..3, 1..4);
        public static readonly NameGenerationParams WorldName = new(1..2, 2..4);
    }
}
