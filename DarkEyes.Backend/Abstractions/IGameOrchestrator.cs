using System;

namespace DarkEyes.Backend.Abstractions
{
    public interface IGameOrchestrator
    {
        public void Dispatch(ICommand command);

        public IObservable<IGameEvent> Events { get; }
    }
}
