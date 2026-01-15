using GameOfLife.API.Services;

namespace GameOfLife.API.Repositories
{
    /// <summary>
    /// Provides an in-memory implementation of the IWorldRepository interface for managing world instances during
    /// application runtime.
    /// </summary>
    /// <remarks>This repository stores world instances in memory and is intended for scenarios such as
    /// testing or development where persistence is not required. All data is lost when the application is stopped. This
    /// class is not thread-safe.
    /// 
    /// THIS IS NOT FOR PRODUCTION AS IT HAS NO PERSISTENCE MECHANISM AND ALSO DOESNT NOT SUPPORT SCALING.
    /// </remarks>
    public class InMemoryWorldRepository : IWorldRepository
    {
        private readonly ILogger _logger;

        private readonly IDictionary<Guid, IWorld> _worlds = new Dictionary<Guid, IWorld>();

        public InMemoryWorldRepository(ILogger<InMemoryWorldRepository> logger)
        {
            _logger = logger;
        }

        public async Task<Guid> CreateWorldAsync(IWorld world)
        {
            Guid identifier = Guid.NewGuid();
            _worlds.Add(identifier, world);

            _logger.LogInformation("Created new world with identifier {0}.", identifier);

            return identifier;
        }

        public async Task<IWorld> RetrieveWorldAsync(Guid worldIdentifier)
        {
            if (_worlds.TryGetValue(worldIdentifier, out IWorld? world))
            {
                return world;
            }

            throw new KeyNotFoundException($"World with identifier {worldIdentifier} not found.");
        }

        public async Task SaveWorldAsync(Guid worldIdentifier, IWorld world)
        {
            // DO NOTHING FOR NOW
        }
    }
}
