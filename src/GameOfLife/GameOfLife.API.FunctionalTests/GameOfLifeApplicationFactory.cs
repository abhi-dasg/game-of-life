using Common.Web.Test;

namespace GameOfLife.API.FunctionalTests
{
    /// <summary>
    /// Provides a test web application factory for the Game of Life application, enabling integration testing with a
    /// configured test host.
    /// </summary>
    /// <remarks>Use this factory to create test server instances for end-to-end or integration tests. The
    /// factory configures the application host and environment to facilitate reliable testing scenarios.</remarks>
    public class GameOfLifeApplicationFactory : TestWebApplicationFactory<Program>
    {
    }
}
