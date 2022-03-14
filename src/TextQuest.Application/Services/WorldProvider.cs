using TextQuest.Application.Interfaces;

namespace TextQuest.Application.Services
{
    internal class WorldProvider : IWorldProvider
    {
        private readonly IRandomGenerator random;
        private readonly INameSetter nameSetter;

        public WorldProvider(IRandomGenerator random, INameSetter nameSetter)
        {
            this.random = random;
            this.nameSetter = nameSetter;
        }

        public World World { get; private set; }

        public void CreateNew(WorldCreationParams creationParams)
        {
            throw new NotImplementedException();
        }
    }
}
