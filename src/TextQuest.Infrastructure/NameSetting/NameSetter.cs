using TextQuest.Application.Interfaces;
using TextQuest.Application.Models;
using TextQuest.Domain.Interfaces;

namespace TextQuest.Infrastructure.NameSetting
{
    internal class NameSetter : INameSetter
    {
        public void SetName(in NameGenerationParams generationParams, INameable nameable)
        {
            throw new NotImplementedException();
        }
    }
}
