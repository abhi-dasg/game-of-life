using GameOfLife.API.Services;

namespace GameOfLife.API.Repositories
{
    /// <summary>
    /// Provides a repository implementation for storing and retrieving world data using a shared cache like AWS ElastiCache
    /// or Azure Redis Cache.
    /// 
    /// THIS WILL BE PREFERRED MECHANISM FOR PRODUCTION USAGE WHERE SCALABILITY IS A REQUIREMENT.
    /// THIS WILL REQUIRE A SERDE MECHANISM FOR EFFICIENTLY READ-WRITE DATA. PROTOBUF IS A GOOD OPTION.
    /// </summary>
    /// <remarks>This repository enables scaling of this service to access and persist world instances through a
    /// shared caching mechanism. It is intended for scenarios where world data needs to be efficiently shared or
    /// synchronized across different components or services. Thread safety and cache consistency depend on the
    /// underlying cache implementation.</remarks>
    public class SharedCacheWorldRepository : IWorldRepository
    {
        /// <inheritdoc/>
        public Task<Guid> CreateWorldAsync(IWorld world)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IWorld> RetrieveWorldAsync(Guid worldIdentifier)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task SaveWorldAsync(Guid worldIdentifier, IWorld world)
        {
            throw new NotImplementedException();
        }
    }
}
