using GameOfLife.API.Models;

namespace GameOfLife.API.Services
{
    /// <summary>
    /// Provides functionality to create and initialize instances of <see cref="IWorld"/> using a collection of living
    /// entities.
    /// </summary>
    /// <remarks>This factory uses dependency injection to obtain <see cref="IWorld"/> implementations. The
    /// created world is initialized asynchronously with the provided entities before being returned.</remarks>
    public class WorldFactory : IWorldFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public WorldFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public async Task<IWorld> BuildWorldAsync(IEnumerable<Entity> livingEntities)
        {
            var world = _serviceProvider.GetRequiredService<IWorld>();
            await world.InitializeAsync(livingEntities);

            return world;
        }
    }
}
