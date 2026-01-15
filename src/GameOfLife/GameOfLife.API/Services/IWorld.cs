using GameOfLife.API.Models;

namespace GameOfLife.API.Services
{
    public interface IWorld
    {
        /// <summary>
        /// Initializes the world with a collection of living entities.
        /// </summary>
        /// <param name="livingEntities"></param>
        /// <returns></returns>
        Task InitializeAsync(IEnumerable<Entity> livingEntities);

        /// <summary>
        /// Evolves the world by one generation
        /// </summary>
        Task<bool> EvolveAsync();

        /// <summary>
        /// Returns a collection of entities that are currently alive.
        /// </summary>
        /// <returns>An enumerable collection of <see cref="Entity"/> objects representing all living entities. The collection
        /// will be empty if no entities are alive.</returns>
        IEnumerable<Entity> GetLivingEntities();
    }
}
