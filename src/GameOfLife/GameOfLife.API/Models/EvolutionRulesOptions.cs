namespace GameOfLife.API.Models
{
    /// <summary>
    /// Configuration options for evolution rules, allowing specification of proximity rules
    /// through configuration or dependency injection.
    /// </summary>
    public class EvolutionRulesOptions
    {
        /// <summary>
        /// Gets or sets the proximity rules that determine whether an entity should remain alive.
        /// </summary>
        public IEnumerable<IProximityRule> StayAliveRules { get; set; } = [];

        /// <summary>
        /// Gets or sets the proximity rules that determine when a new entity is born.
        /// </summary>
        public IEnumerable<IProximityRule> BirthRules { get; set; } = [];
    }
}
