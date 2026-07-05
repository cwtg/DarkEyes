using DarkEyes.Backend.Core.Modifier;

namespace DarkEyes.Backend.Core.Resolution
{
    public interface IPreRollModifier : IModifier
    {
        bool IsActive(in PreRollContext context);
    }
}
