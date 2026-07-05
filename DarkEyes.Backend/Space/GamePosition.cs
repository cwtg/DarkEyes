using System;
using System.Collections.Generic;

namespace DarkEyes.Backend.Space
{
    public record struct GamePosition(int Q, int R, int Level)
    {
        public readonly IReadOnlyList<GamePosition> Neighbors()
    {
        ReadOnlySpan<(int dQ, int dR)> hexDirections =[
        (1, 0), (1, -1), (0, -1),
        (-1, 0), (-1, 1), (0, 1)
        ];

        List<GamePosition> neighbors =[];
        foreach ((int dQ, int dR) in hexDirections)
        {
            neighbors.Add(new GamePosition(Q + dQ, R + dR, Level));
        }

        return neighbors;
    }

    public readonly int DistanceTo(GamePosition position)
    {
        int dq = Q - position.Q;
        int dr = R - position.R;
        int ds = -(dq + dr);
        return Math.Max(Math.Abs(dq), Math.Max(Math.Abs(dr), Math.Abs(ds)));
    }
}
}
