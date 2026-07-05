using DarkEyes.Backend.Core.Resolution;

namespace DarkEyes.Backend.Core.Modifier
{
    public class NeverCondition : IActivationCondition<IPreRollContext>, IActivationCondition<IPostRollContext>
    {
        public bool IsActive(IPreRollContext context) => false;
        public bool IsActive(IPostRollContext context) => false;
    }
}
