namespace GameOfLife.API.Services.Metrics
{
    /// <summary>
    /// Defines methods for publishing birth, death and population count metrics to a monitoring or analytics system.
    /// </summary>
    /// <remarks>Implementations of this interface are responsible for reporting metric data, such as birth
    /// counts, death counts and population counts, to external systems for tracking or analysis.
    /// The specific destination and format of the published metrics depend on the implementation.</remarks>
    public interface IMetricPublisher
    {
        /// <summary>
        /// Publishes the specified birth count asynchronously to the configured destination.
        /// </summary>
        /// <param name="count">The number of births to publish. Must be zero or greater.</param>
        /// <returns>A task that represents the asynchronous publish operation.</returns>
        Task PublishBirthCount(int count);

        /// <summary>
        /// Publishes the specified population count asynchronously to the configured destination.
        /// </summary>
        /// <param name="count">The population count to publish. Must be zero or greater.</param>
        /// <returns>A task that represents the asynchronous publish operation.</returns>
        Task PublishPopulationCount(int count);

        /// <summary>
        /// Publishes the specified death count to the configured destination asynchronously.
        /// </summary>
        /// <param name="count">The number of deaths to publish. Must be zero or greater.</param>
        /// <returns>A task that represents the asynchronous publish operation.</returns>
        Task PublishDeathCount(int count);
    }
}
