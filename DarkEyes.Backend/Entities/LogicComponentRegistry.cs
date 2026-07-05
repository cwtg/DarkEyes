using System;
using System.Collections.Generic;

namespace DarkEyes.Backend.Entities
{
    public class LogicComponentRegistry
    {
        private readonly Dictionary<string, ILogicComponent> _registry =[];

        public void Register(string id, ILogicComponent logicComponent)
        {
            if (!string.IsNullOrWhiteSpace(id))
                _registry.Add(id, logicComponent);
            else
                throw new ArgumentException($"Invalid Id");
        }

        public ILogicComponent Resolve(string id)
        {
            return _registry[id];
        }

        public bool TryResolve(string id, out ILogicComponent? logicComponent)
        {
            return _registry.TryGetValue(id, out logicComponent);
        }
    }
}
