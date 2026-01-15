namespace GameOfLife.API.Configuration
{
    /// <summary>
    /// Configuration for evolution rules that can be deserialized from appsettings.json
    /// </summary>
    public class EvolutionRulesConfiguration
    {
        /// <summary>
        /// Rules that determine whether an entity should remain alive
        /// </summary>
        public List<ProximityRuleConfiguration> StayAliveRules { get; set; } = [];

        /// <summary>
        /// Rules that determine when a new entity is born
        /// </summary>
        public List<ProximityRuleConfiguration> BirthRules { get; set; } = [];
    }
}
