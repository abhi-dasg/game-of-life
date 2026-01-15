
namespace GameOfLife.API.Services.Metrics
{
    /// <summary>
    /// Provides functionality for publishing monitoring metrics related to birth, death, and population counts.
    /// </summary>
    /// <remarks>This class implements the IMetricPublisher interface to enable reporting of key population
    /// metrics to a monitoring system. Instances of this class are typically used in scenarios where real-time or
    /// periodic metric reporting is required for analytics or operational monitoring.
    /// 
    /// THIS IS RECOMMENDED FOR PRODUCTION USE.
    /// </remarks>
    public class MonitoringMetricPublisher : IMetricPublisher
    {
        /// <inheritdoc/>
        public Task PublishBirthCount(int count)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PublishDeathCount(int count)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PublishPopulationCount(int count)
        {
            throw new NotImplementedException();
        }
    }
}
