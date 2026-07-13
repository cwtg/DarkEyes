using System;
using DarkEyes.Backend.Events;
using DarkEyes.Backend.Handlers;

namespace DarkEyes.Backend.Orchestration
{
    public interface IGameOrchestrator
    {
        public void Dispatch(ICommand command);

        public IObservable<IGameEvent> Events { get; }
    }
}
