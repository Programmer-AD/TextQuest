using TextQuest.Domain.Interfaces;

namespace TextQuest.Application.Interfaces
{
    public interface INameSetter
    {
        void SetName(in NameGenerationParams generationParams, INameable nameable);
    }
}
