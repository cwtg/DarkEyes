using System;
using DarkEyes.Backend.Commands;
using DarkEyes.Backend.Events;

namespace DarkEyes.Backend.Rx
{
    public interface IGameOrchestrator
    {
        public void Dispatch(ICommand command);

        public IObservable<IGameEvent> Events { get; }
    }
}
