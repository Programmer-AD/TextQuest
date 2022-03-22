namespace TextQuest.Application.Models
{
    public struct WorldCreationParams
    {
        public Range QuestCount;
        public Range ItemTypeCount;
        public Range MonsterTypeCount;

        public int MaxMonsterDropType;
        public int MaxMonsterDropCount;

        public int MaxMonsterTypeForQuest;
        public int MaxMonsterCountForQuest;

        public int MaxItemForQuestCount;
        public int MaxQuestsForCharacter;
        public int MaxCharactersInLocation;
    }
}
