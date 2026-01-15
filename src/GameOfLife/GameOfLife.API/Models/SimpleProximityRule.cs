namespace GameOfLife.API.Models
{
    /// <summary>
    /// Represents a proximity rule that determines if a specified number of neighbors exactly matches a predefined
    /// value.
    /// </summary>
    public class SimpleProximityRule : IProximityRule
    {
        private readonly byte _checkValue;

        public SimpleProximityRule(byte checkValue)
        {
            if (checkValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(checkValue), "Check value must be non-negative.");
            }

            _checkValue = checkValue;
        }

        /// <inheritdoc/>
        public bool HasSufficientNeighbors(byte neighborCount)
        {
            return neighborCount == _checkValue;
        }
    }
}
