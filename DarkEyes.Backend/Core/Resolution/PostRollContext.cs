namespace DarkEyes.Backend.Core.Resolution
{
    public readonly ref struct PostRollContext(ResultType type, int qualityLevel)
    {
        public readonly ResultType Type = type;
        public readonly int QualityLevel = qualityLevel;
    }
}
