namespace GameOfLife.API.Services.Metrics
{
    /// <summary>
    /// Provides an implementation of <see cref="IMetricPublisher"/> that logs published metric values using the
    /// configured logger.
    /// </summary>
    /// <remarks>This class is intended for scenarios where metric data should be recorded to application logs
    /// for monitoring or diagnostic purposes. Metrics are logged at the Information level. Thread safety and log
    /// formatting are determined by the underlying <see cref="ILogger"/> implementation.</remarks>
    public class LoggingMetricPublisher : IMetricPublisher
    {
        private ILogger _logger;

        public LoggingMetricPublisher(ILogger<LoggingMetricPublisher> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task PublishBirthCount(int count)
        {
            _logger.LogInformation("Birth count: {0}", count);
        }

        /// <inheritdoc/>
        public async Task PublishDeathCount(int count)
        {
            _logger.LogInformation("Death count: {0}", count);
        }

        /// <inheritdoc/>
        public async Task PublishPopulationCount(int count)
        {
            _logger.LogInformation("Population count: {0}", count);
        }
    }
}
