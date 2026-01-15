using GameOfLife.API.Models;

namespace GameOfLife.API.Services.Rules
{
    /// <summary>
    /// Provides configurable evolution rules that can be customized by injecting specific proximity rules
    /// for survival and birth conditions.
    /// </summary>
    /// <remarks>Use this implementation when you need to configure evolution rules dynamically through
    /// dependency injection, rather than using hardcoded rules.</remarks>
    public class ConfigurableEvolutionRules : IEvolutionRules
    {
        public ConfigurableEvolutionRules(
            IEnumerable<IProximityRule> stayAliveRules, 
            IEnumerable<IProximityRule> birthRules)
        {
            StayAliveRules = stayAliveRules ?? throw new ArgumentNullException(nameof(stayAliveRules));
            BirthRules = birthRules ?? throw new ArgumentNullException(nameof(birthRules));
        }

        public IEnumerable<IProximityRule> StayAliveRules { get; }

        public IEnumerable<IProximityRule> BirthRules { get; }
    }
}
