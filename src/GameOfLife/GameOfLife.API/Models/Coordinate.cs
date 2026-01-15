namespace GameOfLife.API.Models
{
    /// <summary>
    /// Represents a two-dimensional point using integer X and Y coordinates.
    /// </summary>
    /// <remarks>Use the Coordinate struct to specify positions or locations in a 2D space, such as grid-based
    /// layouts or geometric calculations. Coordinates are immutable; their values are set during initialization and
    /// cannot be changed after creation.</remarks>
    public struct Coordinate
    {
        public int X { get; init; }
        public int Y { get; init; }

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
