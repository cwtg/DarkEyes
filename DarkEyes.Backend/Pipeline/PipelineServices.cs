using DarkEyes.Backend.Checks;

namespace DarkEyes.Backend.Pipeline;

/// <summary>What a store hands to each chain: the routers and the rule module's services.</summary>
internal sealed record PipelineServices(ActionRouter Actions, EffectRouter Effects, IChecks? Checks);
