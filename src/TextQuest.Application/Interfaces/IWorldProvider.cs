namespace TextQuest.Application.Interfaces
{
    public interface IWorldProvider
    {
        public World World { get; }

        void CreateNew(WorldCreationParams creationParams);
    }
}
