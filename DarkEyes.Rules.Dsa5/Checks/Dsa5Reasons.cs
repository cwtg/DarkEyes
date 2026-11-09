using DarkEyes.Backend;
using DarkEyes.Rules.Dsa5.Characters;

namespace DarkEyes.Rules.Dsa5.Checks;

/// <summary>The DSA5 reasons why a check is not possible.</summary>
public static class Dsa5Reasons
{
    public const string AttributeTooLowCode = "dsa5.attribute_too_low";

    public const string AttributeArgument = "attribute";

    /// <summary>A modifier lowers this attribute to 0 or below, so the check can't be attempted.</summary>
    public static Reason AttributeTooLow(AttributeId attribute) => Reason.Create(AttributeTooLowCode, (AttributeArgument, attribute.Value));
}
