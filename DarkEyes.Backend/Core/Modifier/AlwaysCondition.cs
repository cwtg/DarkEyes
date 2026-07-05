using DarkEyes.Backend.Core.Resolution;

namespace DarkEyes.Backend.Core.Modifier
{
    public sealed class AlwaysCondition : IActivationCondition<IPreRollContext>, IActivationCondition<IPostRollContext>
    {
        public bool IsActive(IPreRollContext context) => true;
        public bool IsActive(IPostRollContext context) => true;
    }
}
