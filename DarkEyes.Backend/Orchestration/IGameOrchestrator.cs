using System;
using DarkEyes.Backend.Events;
using DarkEyes.Backend.Handlers;

namespace DarkEyes.Backend.Orchestration;

public interface IGameOrchestrator
{
    void Dispatch(ICommand command);

    IObservable<IGameEvent> Events { get; }
}
