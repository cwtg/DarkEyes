using DarkEyes.Backend.Core.Resolution;

namespace DarkEyes.Backend.Core.Modifier
{
    public interface IActivationCondition <in T>
    {
        public bool IsActive(T context);
    }
}
