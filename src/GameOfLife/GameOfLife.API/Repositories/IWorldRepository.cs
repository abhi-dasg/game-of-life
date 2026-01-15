using GameOfLife.API.Services;

namespace GameOfLife.API.Repositories
{
    /// <summary>
    /// Defines methods for persisting and retrieving world data.
    /// </summary>
    public interface IWorldRepository
    {
        /// <summary>
        /// Asynchronously creates a new world and returns its unique identifier.
        /// </summary>
        /// <param name="world">The world to be created. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the unique identifier of the
        /// newly created world.</returns>
        Task<Guid> CreateWorldAsync(IWorld world);

        /// <summary>
        /// Saves the specified world to persistent storage.
        /// </summary>
        /// <param name="worldIdentifier">The unique identifier of the world to be saved.</param>
        /// <param name="world">The world instance to be saved. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task SaveWorldAsync(Guid worldIdentifier, IWorld world);

        /// <summary>
        /// Retrieves the world associated with the specified unique identifier.
        /// </summary>
        /// <param name="worldIdentifier">The unique identifier of the world to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the world associated with the
        /// specified identifier.</returns>
        Task<IWorld> RetrieveWorldAsync(Guid worldIdentifier);
    }
}
