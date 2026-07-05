using DarkEyes.Backend.Core.Modifier;

namespace DarkEyes.Backend.Core.Resolution
{
    public interface IPostRollModifier : IModifier
    {
        bool IsActive(in PostRollContext context);
    }
}
