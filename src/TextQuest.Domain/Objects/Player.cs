﻿namespace TextQuest.Domain.Objects
{
    public class Player
    {
        public Location Location { get; set; }
        public HashSet<Quest> Quests { get; set; } = new();
        public CountedCollection<Item> Items { get; set; } = new();
    }
}
