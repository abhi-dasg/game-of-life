using GameOfLife.API.Configuration;
using GameOfLife.API.Models;

namespace GameOfLife.API.Services.Rules
{
    /// <summary>
    /// Factory for creating IProximityRule instances from configuration
    /// </summary>
    public static class ProximityRuleFactory
    {
        /// <summary>
        /// Creates a proximity rule from configuration
        /// </summary>
        public static IProximityRule CreateRule(ProximityRuleConfiguration config)
        {
            ArgumentNullException.ThrowIfNull(config);

            return config.Type.ToLowerInvariant() switch
            {
                "simple" => new SimpleProximityRule(config.Value),
                "range" => new RangeProximityRule(config.Value, config.MaxValue ?? config.Value),
                _ => throw new ArgumentException($"Unknown proximity rule type: {config.Type}", nameof(config))
            };
        }

        /// <summary>
        /// Creates multiple proximity rules from a collection of configurations
        /// </summary>
        public static IEnumerable<IProximityRule> CreateRules(IEnumerable<ProximityRuleConfiguration> configs)
        {
            return configs?.Select(CreateRule) ?? [];
        }
    }
}
