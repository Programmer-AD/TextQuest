namespace TextQuest.Application.Models
{
    public struct WorldCreationParams
    {
        public Range QuestCount;
        public Range ItemTypeCount;
        public Range MonsterTypeCount;

        public Range QuestsForCharacter;
        public Range CharactersInLocation;

        public int MaxMonsterDropType;
        public int MaxMonsterDropCount;

        public int MaxItemCountForQuest;
        public int MaxMonsterTypeForQuest;
        public int MaxMonsterCountForQuest;
    }
}
