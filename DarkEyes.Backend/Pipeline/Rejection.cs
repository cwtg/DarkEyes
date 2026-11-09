using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Pipeline;

/// <summary>
/// Why an action was refused: a stable, act-namespaced reason code plus named arguments.
/// Acts create rejections only through their own factories, which keep code and arguments consistent.
/// Arguments are restricted to simple, deterministic values: <see cref="int"/>, <see cref="bool"/>, <see cref="string"/>, <see cref="EntityId"/>.
/// </summary>
public sealed record Rejection
{
    private Rejection(string code, ImmutableSortedDictionary<string, object> arguments)
    {
        Code = code;
        Arguments = arguments;
    }

    public string Code { get; }

    public ImmutableSortedDictionary<string, object> Arguments { get; }

    public static Rejection Create(string code, params (string Name, object Value)[] arguments)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("A rejection needs a code.", nameof(code));
        }

        ImmutableSortedDictionary<string, object>.Builder builder = ImmutableSortedDictionary.CreateBuilder<string, object>(StringComparer.Ordinal);
        foreach ((string name, object value) in arguments)
        {
            if (value is not (int or bool or string or EntityId))
            {
                throw new ArgumentException($"Rejection argument '{name}' must be an int, bool, string or EntityId, not {value?.GetType().Name ?? "null"}.", nameof(arguments));
            }

            builder.Add(name, value);
        }

        return new Rejection(code, builder.ToImmutable());
    }

    /// <summary>Value equality over code and arguments (a record alone would compare the argument map by reference).</summary>
    public bool Equals(Rejection? other)
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
