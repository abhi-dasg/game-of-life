using GameOfLife.API.Models;

namespace GameOfLife.API.Services.Rules
{
    /// <summary>
    /// Defines the set of proximity-based rules that govern cell survival and birth in an evolutionary simulation.
    /// </summary>
    /// <remarks>Implementations of this interface provide collections of rules that determine whether a cell
    /// remains alive or is born, typically based on the state of neighboring cells. These rules are commonly used in
    /// cellular automata or similar grid-based simulations to control evolution dynamics.</remarks>
    public interface IEvolutionRules
    {
        /// <summary>
        /// Gets the collection of proximity rules that determine whether an entity should remain active.
        /// </summary>
        /// <remarks>Each rule in the collection is evaluated to decide if the entity stays alive based on
        /// proximity conditions. The returned collection is read-only.</remarks>
        IEnumerable<IProximityRule> StayAliveRules { get; }

        /// <summary>
        /// Gets the collection of proximity rules that determine when a new entity is created based on neighboring
        /// conditions.
        /// </summary>
        /// <remarks>Use this property to access the set of rules applied during the birth phase of the
        /// simulation. The returned collection is read-only and reflects the current configuration of birth logic.
        /// Modifying the rules may affect how entities are spawned in response to proximity events.</remarks>
        IEnumerable<IProximityRule> BirthRules { get; }
    }
}
