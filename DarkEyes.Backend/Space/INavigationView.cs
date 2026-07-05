namespace DarkEyes.Backend.Space
{
    public interface INavigationView
    {
        public bool IsWalkable(GamePosition position);

        public bool IsOccupied(GamePosition position);

        public int MovementCost(GamePosition position);

        public CellContext ContextAt(GamePosition position);
    }
}
