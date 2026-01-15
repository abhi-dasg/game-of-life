using GameOfLife.API.Models;
using GameOfLife.API.Services.Metrics;
using GameOfLife.API.Services.Rules;

namespace GameOfLife.API.Services
{
    /// <summary>
    /// Represents a simulation world that manages the state and evolution of living entities according to 
    /// Conways Game of Life rules.
    /// </summary>
    /// <remarks>The World class provides methods to evolve the state of entities and to query the current set
    /// of living entities. It is typically used to implement cellular automata or similar simulations where entities
    /// change state based on their neighbors. This class is now thread-safe for concurrent evolution calls through
    /// internal synchronization using a lock.</remarks>
    public class World : IWorld
    {
        private readonly IEvolutionRules _evolutionRules;
        private readonly IMetricPublisher _metricPublisher;
        private readonly object _evolutionLock = new object();

        private ISet<Entity> _livingEntities = new HashSet<Entity>();

        public World(IEvolutionRules evolutionRules,
                     IMetricPublisher metricPublisher)
        {
            _evolutionRules = evolutionRules;
            _metricPublisher = metricPublisher;
        }

        /// <inheritdoc/>
        public async Task InitializeAsync(IEnumerable<Entity> livingEntities)
        {
            lock (_evolutionLock)
            {
                _livingEntities = new HashSet<Entity>(livingEntities);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> EvolveAsync()
        {
            EvolutionStats stats;

            lock (_evolutionLock)
            {
                IDictionary<Entity, byte> immediateNeighborCount = FindImmediateNeighborsProximityCounts();

                (ISet<Entity> nextLivingEntities, stats) = FindEvolvedAliveEntities(immediateNeighborCount);
                
                _livingEntities = nextLivingEntities;
            }

            await PublishEvolutionMetricsAsync(stats);

            return stats.Evolved;
        }

        /// If performance becomes a concern, we can easily move the dictionary to class level and perform
        /// clean up and recalculation instead of new allocations on each evolution. Keeping it 
        /// in method scope for now for cleanliness.
        /// Additionally, we can consider using Parallel.ForEach if performance for very large population
        /// becomes a concern. Not prematurely optimizing now.
        private IDictionary<Entity, byte> FindImmediateNeighborsProximityCounts()
        {
            IDictionary<Entity, byte> immediateNeighborCount = new Dictionary<Entity, byte>();
            foreach (Entity livingEntity in _livingEntities)
            {
                IEnumerable<Entity> neighboringLocations = livingEntity.GetAllImmediateNeighbors();

                foreach (Entity neighbor in neighboringLocations)
                {
                    if (immediateNeighborCount.TryGetValue(neighbor, out byte value))
                    {
                        immediateNeighborCount[neighbor] = ++value;
                    }
                    else
                    {
                        immediateNeighborCount[neighbor] = 1;
                    }
                }
            }

            return immediateNeighborCount;
        }

        /// If performance becomes a concern, we can easily move the set to class level and perform
        /// swaps instead of new allocations on each evolution. Keeping it in method scope for now
        /// for cleanliness.
        /// Additionally, we can consider using Parallel.ForEach if performance for very large population
        /// becomes a concern. Not prematurely optimizing now.
        private (ISet<Entity>, EvolutionStats) FindEvolvedAliveEntities(IDictionary<Entity, byte> immediateNeighborCount)
        {
            EvolutionStats stats = new();

            ISet<Entity> nextLivingEntities = new HashSet<Entity>();
            foreach (Entity neighborEntity in immediateNeighborCount.Keys)
            {
                byte neighborCount = immediateNeighborCount[neighborEntity];
                if (_livingEntities.Contains(neighborEntity))
                {
                    if (_evolutionRules.StayAliveRules.Any(rule => rule.HasSufficientNeighbors(neighborCount)))
                    {
                        nextLivingEntities.Add(neighborEntity);
                    }
                    else
                    {
                        stats.DeathCount++;
                        stats.Evolved = true;
                    }
                }
                else
                {
                    if (_evolutionRules.BirthRules.Any(rule => rule.HasSufficientNeighbors(neighborCount)))
                    {
                        nextLivingEntities.Add(neighborEntity);
                        stats.BirthCount++;
                        stats.Evolved = true;
                    }
                }
            }

            stats.PopulationCount = nextLivingEntities.Count;

            return (nextLivingEntities, stats);
        }

        private async Task PublishEvolutionMetricsAsync(EvolutionStats stats)
        {
            IEnumerable<Task> publishTasks =
            [
                _metricPublisher.PublishBirthCount(stats.BirthCount),
                _metricPublisher.PublishDeathCount(stats.DeathCount),
                _metricPublisher.PublishPopulationCount(stats.PopulationCount)
            ];

            await Task.WhenAll(publishTasks);
        }

        /// <inheritdoc/>
        public IEnumerable<Entity> GetLivingEntities()
        {
            lock (_evolutionLock)
            {
                return _livingEntities.ToList();
            }
        }
    }
}
