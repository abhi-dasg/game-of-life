using Common.Web.Test;

namespace GameOfLife.API.FunctionalTests
{
    /// <summary>
    /// Provides a base class for functional tests targeting the Game of Life web application.
    /// </summary>
    /// <remarks>This class supplies common setup and configuration for derived test classes, including the
    /// creation of a test-specific application factory. Inherit from this class to implement functional tests that
    /// interact with the Game of Life application's endpoints.</remarks>
    [TestClass]
    public abstract class GameOfLifeFunctionalTests : WebApplicationFunctionalTests<GameOfLifeApplicationFactory, Program>
    {
        /// <inheritdoc/>
        protected override GameOfLifeApplicationFactory BuildApplicationFactory()
        {
            return new GameOfLifeApplicationFactory();
        }
    }
}
