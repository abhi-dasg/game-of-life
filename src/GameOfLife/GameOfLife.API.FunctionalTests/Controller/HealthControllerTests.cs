namespace GameOfLife.API.FunctionalTests.Controller
{
    [TestClass]
    public abstract class HealthControllerTests : GameOfLifeFunctionalTests
    {
        [TestClass]
        public class SimpleHealthTests : HealthControllerTests
        {
            private const string EndpointPath = "/health";
            private const string ExpectedResponseMediaType = "text/plain";
            private const string ExpectedResponseContent = "Healthy";

            private HttpResponseMessage _response;

            [TestInitialize]
            public void SendRequest()
            {
                HttpRequestMessage request = new(HttpMethod.Get, EndpointPath);

                _response = _client.SendAsync(request).Result;
            }

            [TestCleanup]
            public void CleanupResponse()
            {
                _response.Dispose();
            }

            [TestMethod]
            public async Task ShouldReturnOkayResponse()
            {
                // Assert
                _response.EnsureSuccessStatusCode();
                string content = await _response.Content.ReadAsStringAsync();
                Assert.AreEqual("Healthy", content);
            }

            [TestMethod]
            public async Task ShouldReturnTextContent()
            {
                // Assert
                Assert.AreEqual(ExpectedResponseMediaType, _response.Content.Headers.ContentType!.MediaType);
            }

            [TestMethod]
            public async Task ShouldReturnExpectedContent()
            {
                // Assert
                string content = await _response.Content.ReadAsStringAsync();
                Assert.AreEqual(ExpectedResponseContent, content);
            }
        }
    }
}
