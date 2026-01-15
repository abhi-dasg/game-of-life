namespace GameOfLife.API.Models
{
    /// <summary>
    /// Contains statistical information about a world evolution cycle, including birth count, death count,
    /// population count, and whether the world evolved.
    /// </summary>
    public class EvolutionStats
    {
        /// <summary>
        /// Gets or sets the number of entities born during the evolution cycle.
        /// </summary>
        public int BirthCount { get; set; }

        /// <summary>
        /// Gets or sets the number of entities that died during the evolution cycle.
        /// </summary>
        public int DeathCount { get; set; }

        /// <summary>
        /// Gets or sets the total population count after the evolution cycle.
        /// </summary>
        public int PopulationCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the world state changed during the evolution cycle.
        /// </summary>
        public bool Evolved { get; set; }
    }
}
