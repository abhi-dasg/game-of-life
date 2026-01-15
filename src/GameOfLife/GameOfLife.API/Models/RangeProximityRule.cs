namespace GameOfLife.API.Models
{
    /// <summary>
    /// Represents a proximity rule that determines whether a value falls within a specified inclusive range.
    /// </summary>
    /// <remarks>Use this rule to check if a given count or value is within the defined minimum and maximum
    /// bounds. This is commonly used in scenarios where a certain number of neighbors or elements must be present
    /// within a specific range to satisfy a condition.</remarks>
    public class RangeProximityRule : IProximityRule
    {
        private readonly byte _minValue;
        private readonly byte _maxValue;

        public RangeProximityRule(byte minValue, byte maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentException("Minimum value cannot be greater than maximum value.");
            }

            if ((minValue < 0) || (maxValue < 0))
            {
                throw new ArgumentOutOfRangeException("Range values cannot be negative.");
            }

            _minValue = minValue;
            _maxValue = maxValue;
        }

        /// <inheritdoc/>
        public bool HasSufficientNeighbors(byte neighborCount)
        {
            return (neighborCount >= _minValue) && (neighborCount <= _maxValue);
        }
    }
}
