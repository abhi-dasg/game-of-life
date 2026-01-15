using GameOfLife.API.Models;

namespace GameOfLife.API.Services
{
    /// <summary>
    /// Defines a factory for creating world instances with specified entity locations.
    /// </summary>
    public interface IWorldFactory
    {
        /// <summary>
        /// Constructs a new world instance populated with entities.
        /// </summary>
        /// <param name="entityLocations">A collection of entities will be placed in the world. Cannot be null.</param>
        /// <returns>Contains an <see cref="IWorld"/> instance with entities placed at the provided locations.</returns>
        Task<IWorld> BuildWorldAsync(IEnumerable<Entity> entityLocations);
    }
}
