namespace TextQuest.Domain.Entities
{
    public class Recipe
    {
        public IReadOnlyList<Item> RequiredItems { get; set; }
        public IReadOnlyList<Item> ObtainedItems { get; set; }
    }
}
