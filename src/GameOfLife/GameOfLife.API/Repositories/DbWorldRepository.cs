using GameOfLife.API.Services;

namespace GameOfLife.API.Repositories
{
    /// <summary>
    /// Provides an implementation of the IWorldRepository interface that manages world entities using a database-backed
    /// data store.
    /// 
    /// THIS IS GOING TO ENABLE SCALING. BUT THIS POSSIBLY WILL BE SLOWER THAN SHARED CACHE.
    /// THIS WILL REQUIRE A SERDE MECHANISM FOR EFFICIENTLY READ-WRITE DATA. PROTOBUF IS A GOOD OPTION.
    /// </summary>
    /// <remarks>This repository enables asynchronous creation, retrieval, and persistence of world entities.
    /// Instances of this class are typically used to abstract database operations related to world management, allowing
    /// for decoupled and testable application architecture.</remarks>
    public class DbWorldRepository : IWorldRepository
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
