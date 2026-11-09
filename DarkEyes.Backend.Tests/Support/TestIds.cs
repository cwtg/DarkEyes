using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Tests.Support;

internal static class TestIds
{
    public static EntityId Alrik { get; } = new("hero.alrik");

    public static EntityId Brin { get; } = new("hero.brin");

    public static EntityId CellarChest { get; } = new("chest.cellar");

    public static EntityId AtticChest { get; } = new("chest.attic");
}
