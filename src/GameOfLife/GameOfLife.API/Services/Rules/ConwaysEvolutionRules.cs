using GameOfLife.API.Models;

namespace GameOfLife.API.Services.Rules
{
    /// <summary>
    /// Provides the standard evolution rules for Conway's Game of Life, defining the conditions under which cells
    /// survive or are born in each generation.
    /// </summary>
    /// <remarks>This class implements the classic rules of Conway's Game of Life: a cell survives if it has 2
    /// or 3 neighbors, and a new cell is born if it has exactly 3 neighbors. The rules are exposed via the <see
    /// cref="IProximityRule"/> collections for use in grid evolution algorithms.</remarks>
    public class ConwaysEvolutionRules : IEvolutionRules
    {
        private readonly IEnumerable<IProximityRule> _stayAliveRules;
        private readonly IEnumerable<IProximityRule> _birthRules;

        public ConwaysEvolutionRules()
        {
            _stayAliveRules = [ new RangeProximityRule(2, 3) ];

            _birthRules = [ new SimpleProximityRule(3) ];
        }

        public IEnumerable<IProximityRule> StayAliveRules => _stayAliveRules;

        public IEnumerable<IProximityRule> BirthRules => _birthRules;
    }
}
