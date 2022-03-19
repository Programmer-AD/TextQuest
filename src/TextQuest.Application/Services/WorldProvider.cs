﻿using TextQuest.Application.Interfaces;
using TextQuest.Domain.Interfaces;
using TextQuest.Domain.Objects;

namespace TextQuest.Application.Services
{
    internal class WorldProvider : IWorldProvider
    {
        private readonly IRandom random;
        private readonly INameGenerator nameGenerator;

        private readonly HashSet<string> usedNames;

        public WorldProvider(IRandom random, INameGenerator nameGenerator)
        {
            this.random = random;
            this.nameGenerator = nameGenerator;
            usedNames = new();
        }

        public World World { get; private set; }

        public void CreateNew(WorldCreationParams creationParams)
        {
            var quests = CreateQuests(creationParams.QuestCount);
            var items = CreateItems(creationParams.ItemTypeCount);
            DistributeItems(quests, items, creationParams.MaxItemCount);
            var characters = CreateCharacters(quests, creationParams.MaxQuestsForCharacter);
            var locations = CreateLocations(characters, creationParams.MaxCharactersInLocation);
            World = CreateWorld(locations);


            World.Name = GetUnusedName(NameGenerationParamsConstants.WorldName);
            SetNames(locations, NameGenerationParamsConstants.LocationName);
            SetNames(characters, NameGenerationParamsConstants.CharacterName);
            SetNames(items, NameGenerationParamsConstants.ItemName);
        }

        private List<Quest> CreateQuests(Range questCountRange)
        {
            var questCount = random.Next(questCountRange);
            var maxRequired = (int)Math.Sqrt(questCount);

            var quests = new List<Quest>(questCount);
            for (int i = 0; i < questCount; i++)
            {
                var quest = new Quest();
                AddRequiredQuests(quest, quests, maxRequired);
                quests.Add(quest);
            }

            random.Mix(quests);
            return quests;
        }

        private void AddRequiredQuests(Quest quest, List<Quest> previousQuests, int maxRequired)
        {
            maxRequired = Math.Min(maxRequired, previousQuests.Count);
            var requiredCount = random.Next(0..maxRequired);

            var requiredQuests = new HashSet<Quest>();
            for (int i = 0; i < requiredCount; i++)
            {
                var requiredQuest = random.NextElement(previousQuests);
                requiredQuests.Add(requiredQuest);
            }

            quest.RequiredQuests.AddRange(requiredQuests);
        }

        private List<Item> CreateItems(Range itemTypeCountRange)
        {
            var itemTypeCount = random.Next(itemTypeCountRange);
            var items = new List<Item>();

            for (int i = 0; i < itemTypeCount; i++)
            {
                var item = new Item();
                items.Add(item);
            }

            return items;
        }

        private void DistributeItems(List<Quest> quests, List<Item> items, int maxItemCount)
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
        }

        private List<Character> CreateCharacters(List<Quest> quests, int maxQuestsForCharacter)
        {
            var characters = new List<Character>();

            var questQueue = new Queue<Quest>(quests);
            while (questQueue.Count > 0)
            {
                var character = new Character();
                AddCharacterQuests(character, questQueue, maxQuestsForCharacter);
                characters.Add(character);
            }

            random.Mix(characters);
            return characters;
        }

        private void AddCharacterQuests(Character character, Queue<Quest> questQueue, int maxQuestsForCharacter)
        {
            var questCount = random.Next(1..maxQuestsForCharacter);
            questCount = Math.Min(questCount, questQueue.Count);

            character.Quests.Capacity = questCount;
            for (int i = 0; i < questCount; i++)
            {
                var quest = questQueue.Dequeue();
                quest.Giver = character;
                character.Quests.Add(quest);
            }
        }

        private List<Location> CreateLocations(List<Character> characters, int maxCharactersInLocation)
        {
            var locations = new List<Location>();

            var characterQueue = new Queue<Character>(characters);
            while (characterQueue.Count > 0)
            {
                var location = new Location();
                SetLocationCharacters(location, characterQueue, maxCharactersInLocation);
                locations.Add(location);
            }

            random.Mix(locations);
            return locations;
        }

        private void SetLocationCharacters(Location location, Queue<Character> characterQueue, int maxCharactersInLocation)
        {
            var characterCount = random.Next(1..maxCharactersInLocation);
            characterCount = Math.Min(characterCount, characterQueue.Count);

            location.Characters.Capacity = characterCount;
            for (int i = 0; i < characterCount; i++)
            {
                var character = characterQueue.Dequeue();
                character.Location = location;
                location.Characters.Add(character);
            }
        }

        private World CreateWorld(List<Location> locations)
        {
            var world = new World();
            world.Locations.AddRange(locations);
            return world;
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
    }
}
