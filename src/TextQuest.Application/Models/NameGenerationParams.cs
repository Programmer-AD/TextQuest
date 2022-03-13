namespace TextQuest.Application.Models
{
    public readonly struct NameGenerationParams
    {
        public readonly ushort MinWordCount, MaxWordCount;
        public readonly ushort MinPartCount, MaxPartCount;
    }
}
