using GameOfLife.API.Models;

namespace GameOfLife.API.Services
{
    /// <summary>
    /// Defines methods for evolving the Game of Life world and retrieving living entities within it.
    /// </summary>
    /// <remarks>This interface provides asynchronous operations for advancing the simulation and querying the
    /// state of living entities. Implementations are expected to handle concurrent calls safely. The coordinate system
    /// used for area queries should be consistent with the world representation.</remarks>
    public interface IGameOfLifeService
    {
        Task<Guid> CreateWorldAsync(IEnumerable<Coordinate> entityLocations, CancellationToken cancellationToken = default);

        /// <summary>
        /// Advances the simulation by the specified number of generations asynchronously.
        /// </summary>
        /// <param name="worldIdentifier">The unique identifier of the world to evolve.</param>
        /// <param name="generationCount">The number of generations to evolve the world. Must be greater than zero. Defaults to 1 if not specified.</param>
        /// <returns>Evolution operation.</returns>
        Task<IEnumerable<Coordinate>> EvolveWorldAsync(Guid worldIdentifier, int generationCount = 1, CancellationToken cancellationToken = default);

        /// <summary>
        /// Evolves to final state and returns set of coordinates representing the world state for the specified world
        /// identifier.
        /// </summary>
        /// <param name="worldIdentifier">The unique identifier of the world whose final state is to be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of coordinates
        /// representing the final state of the world. The collection is empty if the world has no coordinates.</returns>
        Task<IEnumerable<Coordinate>> EvolveToFinalWorldStateAsync(Guid worldIdentifier, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all entities that are currently considered living.
        /// WARNING: Should NOT implement this for very large world with heavy population.
        /// </summary>
        /// <param name="worldIdentifier">The unique identifier of the world to query for living entities.</param>
        /// <returns>An enumerable collection of <see cref="Entity"/> objects representing all 
        /// living entities. If no entities are living, the collection will be
        /// empty.</returns>
        Task<IEnumerable<Coordinate>> GetAllLivingEntitiesAsync(Guid worldIdentifier, CancellationToken cancellationToken = default);

        // TODO: Add methods to get entities within specified region

        /// <summary>
        /// Asynchronously retrieves all living entities located within the rectangular area defined by the specified
        /// corner coordinates.
        /// </summary>
        /// <param name="worldIdentifier">The unique identifier of the world to query for living entities.</param>
        /// <param name="fromCorner">The coordinate representing one corner of the area to search for living entities.</param>
        /// <param name="toCorner">The coordinate representing the opposite corner of the area to search for living entities.</param>
        /// <returns>A collection of living entities found within the specified area. The collection will be empty 
        /// if no living entities are present.</returns>
        Task<IEnumerable<Coordinate>> GetLivingEntitiesInAreaAsync(Guid worldIdentifier, Coordinate fromCorner, Coordinate toCorner, CancellationToken cancellationToken = default);
    }
}
