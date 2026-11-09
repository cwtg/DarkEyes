using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend;

/// <summary>
/// A stable, namespaced reason code plus named arguments: why an action was rejected, or why a check is not possible.
/// The frontend maps codes to texts. Reasons are created only through factories next to their act or rule, which keep
/// code and arguments consistent. Arguments are simple, deterministic values: <see cref="int"/>, <see cref="bool"/>,
/// <see cref="string"/>, <see cref="EntityId"/>.
/// </summary>
public sealed record Reason
{
    private Reason(string code, ImmutableSortedDictionary<string, object> arguments)
    {
        Code = code;
        Arguments = arguments;
    }

    public string Code { get; }

    public ImmutableSortedDictionary<string, object> Arguments { get; }

    public static Reason Create(string code, params (string Name, object Value)[] arguments)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("A reason needs a code.", nameof(code));
        }

        ImmutableSortedDictionary<string, object>.Builder builder = ImmutableSortedDictionary.CreateBuilder<string, object>(StringComparer.Ordinal);
        foreach ((string name, object value) in arguments)
        {
            if (value is not (int or bool or string or EntityId))
            {
                throw new ArgumentException($"Reason argument '{name}' must be an int, bool, string or EntityId, not {value?.GetType().Name ?? "null"}.", nameof(arguments));
            }

            builder.Add(name, value);
        }

        return new Reason(code, builder.ToImmutable());
    }

    /// <summary>Value equality over code and arguments (a record alone would compare the argument map by reference).</summary>
    public bool Equals(Reason? other)
    {
        return other is not null
            && string.Equals(Code, other.Code, StringComparison.Ordinal)
            && Arguments.SequenceEqual(other.Arguments);
    }

    public override int GetHashCode()
    {
        HashCode hash = default;
        hash.Add(Code, StringComparer.Ordinal);
        foreach (KeyValuePair<string, object> argument in Arguments)
        {
            hash.Add(argument.Key, StringComparer.Ordinal);
            hash.Add(argument.Value);
        }

        return hash.ToHashCode();
    }

    /// <summary>For logs and test output, e.g. <c>open_container.too_far_away(distance=3, max=1)</c>.</summary>
    public override string ToString()
    {
        IEnumerable<string> arguments = Arguments.Select(argument => FormattableString.Invariant($"{argument.Key}={argument.Value}"));
        return $"{Code}({string.Join(", ", arguments)})";
    }
}
