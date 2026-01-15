using GameOfLife.API.Controllers;

namespace GameOfLife.API.UnitTests.Controllers
{
    [TestClass]
    public abstract class HealthControllerTests
    {
        private HealthController _controller;

        [TestInitialize]
        public void Setup()
        {
            _controller = new HealthController();
        }

        [TestClass]
        public class SimpleHealthTests : HealthControllerTests
        {
            private const string ExpectedHealthyResponse = "Healthy";

            [TestMethod]
            public async Task ShouldReturnsHealthyResponse()
            {
                // Act
                string result = await _controller.GetSimpleHealth();

                // Assert
                Assert.AreEqual(ExpectedHealthyResponse, result);
            }
        }
    }
}
