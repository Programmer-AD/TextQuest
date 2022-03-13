using TextQuest.Application.Interfaces;

namespace TextQuest.Application.Services
{
    internal class WorldProvider : IWorldProvider
    {
        private readonly INameSetter nameSetter;

        public WorldProvider(INameSetter nameSetter)
        {
            this.nameSetter = nameSetter;
        }

        public World World { get; private set; }

        public void CreateNew()
        {
            throw new NotImplementedException();
        }
    }
}
