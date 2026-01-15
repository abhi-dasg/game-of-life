namespace GameOfLife.API.Models
{
    public struct Entity : IEquatable<Entity>
    {
        public readonly int XCoordinate;
        public readonly int YCoordinate;

        public Entity(int xCoordinate, int yCoordinate)
        {
            this.XCoordinate = xCoordinate;
            this.YCoordinate = yCoordinate;
        }

        public bool Equals(Entity other)
        {
            return (this.XCoordinate == other.XCoordinate) &&
                   (this.YCoordinate == other.YCoordinate);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.XCoordinate, this.YCoordinate);
        }

        public IEnumerable<Entity> GetAllImmediateNeighbors()
        {
            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                for (int yOffset = -1; yOffset <= 1; yOffset++)
                {
                    if ((xOffset == 0) && (yOffset == 0))
                    {
                        continue;
                    }

                    yield return new Entity(this.XCoordinate + xOffset, this.YCoordinate + yOffset);
                }
            }
        }
    }
}
