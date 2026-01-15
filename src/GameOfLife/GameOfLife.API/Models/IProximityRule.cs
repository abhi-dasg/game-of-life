namespace GameOfLife.API.Models
{
    /// <summary>
    /// Defines a rule for determining whether a given number of neighbors meets a proximity condition.
    /// </summary>
    public interface IProximityRule
    {
        /// <summary>
        /// Determines whether the specified number of neighbors meets the required condition.
        /// </summary>
        /// <param name="neighborCount">The number of neighboring elements to evaluate. Must be a non-negative value.</param>
        /// <returns>true if the neighbor count meets criteria; otherwise, false.</returns>
        bool HasSufficientNeighbors(byte neighborCount);
    }
}
