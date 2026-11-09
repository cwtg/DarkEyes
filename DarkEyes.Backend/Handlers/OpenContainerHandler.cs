#pragma warning disable CS8602 // Legacy code, replaced or moved to the rule module in story 01 (docs/implementation/01-open-chest.md).

using System.Collections.Generic;
using DarkEyes.Backend.Core.Modifiers;
using DarkEyes.Backend.Core.Resolution;
using DarkEyes.Backend.Core.Resolution.Outcomes;
using DarkEyes.Backend.Core.Stats;
using DarkEyes.Backend.Entities;
using DarkEyes.Backend.Entities.State;
using DarkEyes.Backend.Events;
using DarkEyes.Backend.Orchestration;

namespace DarkEyes.Backend.Handlers;

public class OpenContainerHandler : ICommandHandler<OpenContainerCommand>
{
    public void Handle(OpenContainerCommand command, HandlerContext context)
    {
        LockedContainerState? containerState = default;
        CharacterStats? actorStats = default;

        // Validate
        try
        {
            Entity container = context.Store.ActiveLocation.Entities.Get(command.ContainerId);
            Entity actor = context.Store.ActiveLocation.Entities.Get(command.ActorId);

            if (actor.Position.DistanceTo(container.Position) > 1)
            {
                context.Emit(new ErrorRaisedEvent("Actor is too far away from container"));
                return;
            }

            if (!container.States.TryGetValue(typeof(LockedContainerState), out object? containerStateObj))
            {
                context.Emit(new ErrorRaisedEvent("Container does not have a LockedContainerState"));
                return;
            }
            containerState = containerStateObj as LockedContainerState;
            if (!containerState.Locked)
            {
                context.Emit(new ErrorRaisedEvent("Container already unlocked"));
                return;
            }

            if (!actor.States.TryGetValue(typeof(CharacterStats), out object? actorStatsObj))
            {
                context.Emit(new ErrorRaisedEvent("Actor does not have a CharacterStats"));
                return;
            }
            actorStats = actorStatsObj as CharacterStats;
        }
        catch (KeyNotFoundException)
        {
            context.Emit(new ErrorRaisedEvent("EntityId not valid"));
            return;
        }

        // Handle
        SkillCheck check = new();
        Skill skill = actorStats!.Skills.GetValueOrDefault(new StatId("lockpicking"));
        var attributes = new HashSet<Attribute>(3);
        var modifiers = new List<IModifier>(3);

        if (attributes.Add(skill.Attribute1))
        {
            modifiers.Add(new ChallengeRatingModifier(skill.Attribute1.Id, containerState!.ChallengeRating));
        }

        if (attributes.Add(skill.Attribute2))
        {
            modifiers.Add(new ChallengeRatingModifier(skill.Attribute2.Id, containerState!.ChallengeRating));
        }

        if (attributes.Add(skill.Attribute3))
        {
            modifiers.Add(new ChallengeRatingModifier(skill.Attribute3.Id, containerState!.ChallengeRating));
        }

        ResolutionRequest resolutionRequest = new(skill, modifiers);
        ResolutionResult result = check.Resolve(resolutionRequest, context.Random);
        context.Emit(new ResolutionRolledEvent(command.ActorId, result.Type, result.QualityLevel));

        // Outcome
        ChestOutcomeMapping outcomeMapping = new();
        if (outcomeMapping.Map(result))
        {
            LockedContainerState unlockedContainer = containerState!.Unlock();
            context.Store.ActiveLocation.Entities.MutateEntity(command.ContainerId, entity => entity.WithState(unlockedContainer));
            context.Emit(new ContainerUnlockEvent());
        }
        else
        {
            context.Emit(new ContainerOpenFailedEvent());
        }
    }
}
