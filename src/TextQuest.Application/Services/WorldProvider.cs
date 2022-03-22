using TextQuest.Application.Interfaces;
using TextQuest.Domain.Interfaces;

namespace TextQuest.Application.Services
{
    internal class WorldProvider : IWorldProvider
    {
        private readonly IRandom random;
        private readonly INameGenerator nameGenerator;

        private readonly List<Quest> quests = new();
        private readonly List<Item> items = new();
        private readonly List<Monster> monsters = new();
        private readonly List<Character> characters = new();
        private readonly List<Location> locations = new();
        private readonly HashSet<string> usedNames = new();

        public WorldProvider(IRandom random, INameGenerator nameGenerator)
        {
            this.random = random;
            this.nameGenerator = nameGenerator;
        }

        public World World { get; private set; }

        public void CreateNew(WorldCreationParams creationParams)
        {
            MakeObjects(creationParams);
            SetupObjects(creationParams);
            SetObjectNames();
            Clear();
        }

        private void MakeObjects(WorldCreationParams creationParams)
        {
            MakeRandomList(quests, creationParams.QuestCount);
            MakeRandomList(items, creationParams.ItemTypeCount);
            MakeRandomList(monsters, creationParams.MonsterTypeCount);

            var characterCount = GetDistributionRange(creationParams.QuestsForCharacter, quests.Count);
            MakeRandomList(characters, characterCount);

            var locationCount = GetDistributionRange(creationParams.CharactersInLocation, characters.Count);
            MakeRandomList(locations, locationCount);

            MakeWorld();
        }

        private static Range GetDistributionRange(Range countLimit, int maxCount)
        {
            return (maxCount / countLimit.End.Value + 1)..(maxCount / countLimit.Start.Value);
        }

        private void MakeRandomList<T>(List<T> list, Range countRange)
            where T : class, new()
        {
            var count = random.Next(countRange);
            list.Capacity = count;

            for (int i = 0; i < count; i++)
            {
                var obj = new T();
                list.Add(obj);
            }
        }

        private void MakeWorld()
        {
            World = new();
            World.Locations.AddRange(locations);
        }

        private void SetupObjects(WorldCreationParams creationParams)
        {
            var maxRequiredQuests = (int)Math.Sqrt(characters.Count);
            SetQuestRequiredQuests(maxRequiredQuests);
            SetMonsterDrops(creationParams.MaxMonsterDropType,
                creationParams.MaxMonsterDropCount);
            SetQuestItems(creationParams.MaxItemCountForQuest,
                creationParams.MaxMonsterTypeForQuest,
                creationParams.MaxMonsterTypeForQuest);
            SetCharacterQuests(creationParams.QuestsForCharacter);
            SetLocationCharacters(creationParams.CharactersInLocation);
            SetMonsterLocations();
        }

        private void SetQuestRequiredQuests(int maxRequired)
        {
            var previousQuests = new List<Quest>();
            foreach (var quest in quests)
            {
                var thisMaxRequired = Math.Min(maxRequired, previousQuests.Count);
                var requiredCount = random.Next(0..thisMaxRequired);

                var requiredQuests = new HashSet<Quest>();
                for (int i = 0; i < requiredCount; i++)
                {
                    var requiredQuest = random.NextElement(previousQuests);
                    requiredQuests.Add(requiredQuest);
                }

                quest.RequiredQuests.AddRange(requiredQuests);
                previousQuests.Add(quest);
            }

            random.Mix(quests);
        }

        private void SetMonsterDrops(int maxDropTypeCount, int maxDropCount)
        {
            foreach (var monster in monsters)
            {
                var dropTypeCount = random.Next(1..maxDropTypeCount);

                for (int i = 0; i < dropTypeCount; i++)
                {
                    var dropCount = (uint)random.Next(1..maxDropCount);
                    var item = random.NextElement(items);
                    monster.DroppedItems.Add(new(item, dropCount));
                }
            }
        }

        private void SetQuestItems(int maxItemCount, int maxMonsterTypeForQuest, int maxMonsterCountForQuest)
        {
            foreach (var toQuest in quests)
            {
                foreach (var fromQuest in toQuest.RequiredQuests)
                {
                    var item = random.NextElement(items);
                    var count = (uint)random.Next(1..maxItemCount);
                    var countedItem = new Counted<Item>(item, count);

                    fromQuest.ObtainedItems.Add(countedItem);
                    toQuest.RequiredItems.Add(countedItem);
                }
            }
            foreach (var quest in quests)
            {
                var monsterTypeCount = random.Next(1..maxMonsterTypeForQuest);
                for (int i = 0; i < monsterTypeCount; i++)
                {
                    var monster = random.NextElement(monsters);
                    var monsterCount = (uint)random.Next(1..maxMonsterCountForQuest);
                    foreach (var item in monster.DroppedItems)
                    {
                        var requiredCount = item.Count * monsterCount;
                        quest.RequiredItems.Add(new(item.Value, requiredCount));
                    }
                }
            }
        }

        private void SetCharacterQuests(Range questsForCharacter)
        {
            int pos = 0;
            for (int i = 0; i < questsForCharacter.Start.Value && pos < quests.Count; i++)
            {
                foreach (var character in characters)
                {
                    var quest = quests[pos++];
                    character.Quests.Add(quest);
                    quest.Giver = character;
                    if (pos >= quests.Count)
                    {
                        break;
                    }
                }
            }
            while (pos < quests.Count)
            {
                var character = random.NextElement(characters);
                if (character.Quests.Count < questsForCharacter.End.Value)
                {
                    var quest = quests[pos++];
                    character.Quests.Add(quest);
                    quest.Giver = character;
                }
            }
        }

        private void SetLocationCharacters(Range charactersInLocation)
        {
            int pos = 0;
            for (int i = 0; i < charactersInLocation.Start.Value && pos < characters.Count; i++)
            {
                foreach (var location in locations)
                {
                    var character = characters[pos++];
                    location.Characters.Add(character);
                    character.Location = location;
                    if (pos >= characters.Count)
                    {
                        break;
                    }
                }
            }
            while (pos < characters.Count)
            {
                var location = random.NextElement(locations);
                if (location.Characters.Count < charactersInLocation.End.Value)
                {
                    var character = characters[pos++];
                    location.Characters.Add(character);
                    character.Location = location;
                }
            }
        }

        private void SetMonsterLocations()
        {
            foreach (var monster in monsters)
            { 
                var location = random.NextElement(locations);
                location.Monsters.Add(monster);
            }
            foreach (var location in locations)
            {
                foreach (var monster in monsters)
                {
                    if (random.Next(1..4) == 1 && !location.Monsters.Contains(monster))
                    {
                        location.Monsters.Add(monster);
                    }
                }
            }
        }

        private void SetObjectNames()
        {
            SetNames(items, NameGenerationParamsConstants.ItemName);
            SetNames(monsters, NameGenerationParamsConstants.MonsterName);
            SetNames(characters, NameGenerationParamsConstants.CharacterName);
            SetNames(locations, NameGenerationParamsConstants.LocationName);
            World.Name = GetUnusedName(NameGenerationParamsConstants.WorldName);
        }

        private void SetNames(IEnumerable<INameable> nameables, NameGenerationParams nameParams)
        {
            foreach (var nameable in nameables)
            {
                var name = GetUnusedName(nameParams);
                nameable.Name = name;
            }
        }

        private string GetUnusedName(NameGenerationParams nameParams)
        {
            string name;
            do
            {
                name = nameGenerator.GetName(nameParams);
            } while (usedNames.Contains(name));

            usedNames.Add(name);
            return name;
        }

        private void Clear()
        {
            quests.Clear();
            items.Clear();
            monsters.Clear();
            characters.Clear();
            locations.Clear();
            usedNames.Clear();
        }
    }
}
