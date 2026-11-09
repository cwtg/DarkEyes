namespace DarkEyes.Backend.Space;

public interface INavigationView
{
    bool IsWalkable(GamePosition position);

    bool IsOccupied(GamePosition position);

    int MovementCost(GamePosition position);

    CellContext ContextAt(GamePosition position);
}
