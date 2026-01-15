namespace GameOfLife.API.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a population cannot be stabilized during world evolution process.
    /// </summary>
    /// <remarks>This exception typically indicates that the algorithm was unable to reach a stable state for
    /// the population within the expected parameters or constraints. Handle this exception to detect and respond to
    /// scenarios where stabilization is not possible, such as by adjusting input parameters or terminating the
    /// operation gracefully.</remarks>
    public class UnableToStabilizePopulationException : Exception
    {
        public int MaxGenerations { get; }

        public UnableToStabilizePopulationException(int maxGenerations)
            : base($"Population could not stabilize within {maxGenerations} generations.")
        {
            MaxGenerations = maxGenerations;
        }

        public UnableToStabilizePopulationException(int maxGenerations, string message)
            : base(message)
        {
            MaxGenerations = maxGenerations;
        }

        public UnableToStabilizePopulationException(int maxGenerations, string message, Exception innerException)
            : base(message, innerException)
        {
            MaxGenerations = maxGenerations;
        }

        public string GetErrorCode() => "PopulationStabilizationFailed";
    }
}
