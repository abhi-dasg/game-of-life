using AutoMapper;
using GameOfLife.API.Configuration;
using GameOfLife.API.Exceptions;
using GameOfLife.API.Models;
using GameOfLife.API.Repositories;
using Microsoft.Extensions.Options;

namespace GameOfLife.API.Services
{
    /// <inheritdoc/>
    public class GameOfLifeService : IGameOfLifeService
    {
        private readonly IWorldFactory _worldFactory;
        private readonly IWorldRepository _worldRepository;
        private readonly IMapper _mapper;
        private readonly GameOfLifeConfiguration _configuration;

        public GameOfLifeService(IWorldFactory worldFactory,
                                 IWorldRepository worldRepository,
                                 IMapper mapper,
                                 IOptions<GameOfLifeConfiguration> configuration)
        {
            _worldFactory = worldFactory;
            _worldRepository = worldRepository;
            _mapper = mapper;
            _configuration = configuration.Value;
        }

        public async Task<Guid> CreateWorldAsync(IEnumerable<Coordinate> entityLocations, CancellationToken cancellationToken = default)
        {
            IEnumerable<Entity> livingEntities = _mapper.Map<IEnumerable<Entity>>(entityLocations);
            IWorld newWorld = await _worldFactory.BuildWorldAsync(livingEntities);

            return await _worldRepository.CreateWorldAsync(newWorld);
        }

        public async Task<IEnumerable<Coordinate>> EvolveWorldAsync(Guid worldIdentifier, int generationCount = 1, CancellationToken cancellationToken = default)
        {
            IWorld world = await _worldRepository.RetrieveWorldAsync(worldIdentifier);
            for (int i = 0; i < generationCount; i++)
            {
                bool evolved = await world.EvolveAsync();
                // This can significantly impact performance. So be careful about persistence strategy.
                await _worldRepository.SaveWorldAsync(worldIdentifier, world);

                if (!evolved)
                {
                    break;
                }
            }

            return _mapper.Map<IEnumerable<Coordinate>>(world.GetLivingEntities());
        }

        public async Task<IEnumerable<Coordinate>> GetAllLivingEntitiesAsync(Guid worldIdentifier, CancellationToken cancellationToken = default)
        {
            IWorld world = await _worldRepository.RetrieveWorldAsync(worldIdentifier);

            return _mapper.Map<IEnumerable<Coordinate>>(world.GetLivingEntities());
        }

        public async Task<IEnumerable<Coordinate>> EvolveToFinalWorldStateAsync(Guid worldIdentifier, CancellationToken cancellationToken = default)
        {
            IWorld world = await _worldRepository.RetrieveWorldAsync(worldIdentifier);

            bool evolved;
            int generations = 0;

            do
            {
                evolved = await world.EvolveAsync();
                // This can significantly impact performance. So be careful about persistence strategy.
                await _worldRepository.SaveWorldAsync(worldIdentifier, world);

                generations++;
                if (generations >= _configuration.MaxAutoEvolution)
                {
                    throw new UnableToStabilizePopulationException(_configuration.MaxAutoEvolution);
                }
            } while (evolved);

            return _mapper.Map<IEnumerable<Coordinate>>(world.GetLivingEntities());
        }

        public async Task<IEnumerable<Coordinate>> GetLivingEntitiesInAreaAsync(Guid worldIdentifier, Coordinate fromCorner, Coordinate toCorner, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
