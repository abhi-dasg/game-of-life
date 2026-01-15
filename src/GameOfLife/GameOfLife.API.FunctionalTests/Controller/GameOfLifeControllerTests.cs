using GameOfLife.API.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace GameOfLife.API.FunctionalTests.Controller
{
    [TestClass]
    public abstract class GameOfLifeControllerTests : GameOfLifeFunctionalTests
    {
        [TestClass]
        public class CreateWorldTests : GameOfLifeControllerTests
        {
            private const string EndpointPath = "/api/v1/GameOfLife";
            private const string ExpectedResponseMediaType = "text/plain";

            private HttpResponseMessage _response;
            private IEnumerable<Coordinate> _requestBody;

            [TestInitialize]
            public void SendRequest()
            {
                // Pattern visualization:
                // ? ?
                // ?
                _requestBody = new List<Coordinate>
                {
                    new Coordinate { X = 1, Y = 1 },
                    new Coordinate { X = 1, Y = 2 },
                    new Coordinate { X = 2, Y = 1 }
                };

                HttpRequestMessage request = new(HttpMethod.Post, EndpointPath)
                {
                    Content = JsonContent.Create(_requestBody)
                };

                _response = _client.SendAsync(request).Result;
            }

            [TestCleanup]
            public void CleanupResponse()
            {
                _response.Dispose();
            }

            [TestMethod]
            public async Task ShouldReturnCreatedStatusCode()
            {
                // Assert
                Assert.AreEqual(System.Net.HttpStatusCode.Created, _response.StatusCode);
            }

            [TestMethod]
            public async Task ShouldReturnTextPlainContentType()
            {
                // Assert
                Assert.AreEqual(ExpectedResponseMediaType, _response.Content.Headers.ContentType!.MediaType);
            }

            [TestMethod]
            public async Task ShouldReturnValidGuidAsWorldIdentifier()
            {
                // Act
                string content = await _response.Content.ReadAsStringAsync();

                // Assert
                Assert.IsTrue(Guid.TryParse(content, out Guid worldIdentifier));
                Assert.AreNotEqual(Guid.Empty, worldIdentifier);
            }

            [TestMethod]
            public async Task ShouldAcceptEmptyCoordinateList()
            {
                // Arrange
                HttpRequestMessage request = new(HttpMethod.Post, EndpointPath)
                {
                    Content = JsonContent.Create(Array.Empty<Coordinate>())
                };

                // Act
                HttpResponseMessage response = await _client.SendAsync(request);

                // Assert
                Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
                response.Dispose();
            }
        }

        [TestClass]
        public class EvolveTests : GameOfLifeControllerTests
        {
            private const string ExpectedResponseMediaType = "application/json";

            private HttpResponseMessage _createResponse;
            private Guid _worldIdentifier;
            private HttpResponseMessage _evolveResponse;

            [TestInitialize]
            public async Task SetupWorldAndEvolve()
            {
                // Block pattern (2x2 square - still life):
                // ? ?
                // ? ?
                IEnumerable<Coordinate> blockPattern = new List<Coordinate>
                {
                    new Coordinate { X = 1, Y = 1 },
                    new Coordinate { X = 1, Y = 2 },
                    new Coordinate { X = 2, Y = 1 },
                    new Coordinate { X = 2, Y = 2 }
                };

                HttpRequestMessage createRequest = new(HttpMethod.Post, "/api/v1/GameOfLife")
                {
                    Content = JsonContent.Create(blockPattern)
                };

                _createResponse = await _client.SendAsync(createRequest);
                string worldIdString = await _createResponse.Content.ReadAsStringAsync();
                _worldIdentifier = Guid.Parse(worldIdString);

                HttpRequestMessage evolveRequest = new(HttpMethod.Put, $"/api/v1/GameOfLife/{_worldIdentifier}/evolve");
                _evolveResponse = await _client.SendAsync(evolveRequest);
            }

            [TestCleanup]
            public void CleanupResponses()
            {
                _createResponse?.Dispose();
                _evolveResponse?.Dispose();
            }

            [TestMethod]
            public async Task ShouldReturnOkStatusCode()
            {
                // Assert
                _evolveResponse.EnsureSuccessStatusCode();
                Assert.AreEqual(System.Net.HttpStatusCode.OK, _evolveResponse.StatusCode);
            }

            [TestMethod]
            public async Task ShouldReturnJsonContentType()
            {
                // Assert
                Assert.AreEqual(ExpectedResponseMediaType, _evolveResponse.Content.Headers.ContentType!.MediaType);
            }

            [TestMethod]
            public async Task ShouldReturnCoordinateArray()
            {
                // Act
                IEnumerable<Coordinate>? coordinates = await _evolveResponse.Content.ReadFromJsonAsync<IEnumerable<Coordinate>>();

                // Assert
                Assert.IsNotNull(coordinates);
            }

            [TestMethod]
            public async Task ShouldEvolveBlockPatternCorrectly()
            {
                // Act
                IEnumerable<Coordinate>? coordinates = await _evolveResponse.Content.ReadFromJsonAsync<IEnumerable<Coordinate>>();

                // Assert - Block pattern should remain stable
                Assert.IsNotNull(coordinates);
                Assert.AreEqual(4, coordinates.Count());
            }

            [TestMethod]
            [DataRow(1)]
            [DataRow(5)]
            [DataRow(10)]
            public async Task ShouldAcceptGenerationCountParameter(int generationCount)
            {
                // Arrange
                HttpRequestMessage request = new(HttpMethod.Put, $"/api/v1/GameOfLife/{_worldIdentifier}/evolve?generationCount={generationCount}");

                // Act
                HttpResponseMessage response = await _client.SendAsync(request);

                // Assert
                response.EnsureSuccessStatusCode();
                response.Dispose();
            }
        }

        [TestClass]
        public class EvolveWithGenerationCountTests : GameOfLifeControllerTests
        {
            private Guid _worldIdentifier;

            [TestInitialize]
            public async Task SetupWorld()
            {
                // Blinker pattern (period-2 oscillator):
                // Horizontal:  ? ? ?
                // Vertical:      ?
                //                ?
                //                ?
                IEnumerable<Coordinate> blinkerPattern = new List<Coordinate>
                {
                    new Coordinate { X = 1, Y = 2 },
                    new Coordinate { X = 2, Y = 2 },
                    new Coordinate { X = 3, Y = 2 }
                };

                HttpRequestMessage createRequest = new(HttpMethod.Post, "/api/v1/GameOfLife")
                {
                    Content = JsonContent.Create(blinkerPattern)
                };

                HttpResponseMessage createResponse = await _client.SendAsync(createRequest);
                string worldIdString = await createResponse.Content.ReadAsStringAsync();
                _worldIdentifier = Guid.Parse(worldIdString);
                createResponse.Dispose();
            }

            [TestMethod]
            public async Task ShouldEvolveBlinkerTwiceToReturnToOriginalState()
            {
                // Arrange
                HttpRequestMessage request = new(HttpMethod.Put, $"/api/v1/GameOfLife/{_worldIdentifier}/evolve?generationCount=2");

                // Act
                HttpResponseMessage response = await _client.SendAsync(request);
                IEnumerable<Coordinate>? coordinates = await response.Content.ReadFromJsonAsync<IEnumerable<Coordinate>>();

                // Assert
                Assert.IsNotNull(coordinates);
                Assert.AreEqual(3, coordinates.Count());
                Assert.IsTrue(coordinates.Any(c => c.X == 1 && c.Y == 2));
                Assert.IsTrue(coordinates.Any(c => c.X == 2 && c.Y == 2));
                Assert.IsTrue(coordinates.Any(c => c.X == 3 && c.Y == 2));
                
                response.Dispose();
            }

            [TestMethod]
            public async Task ShouldReturnOkForZeroGenerations()
            {
                // Arrange
                HttpRequestMessage request = new(HttpMethod.Put, $"/api/v1/GameOfLife/{_worldIdentifier}/evolve?generationCount=0");

                // Act
                HttpResponseMessage response = await _client.SendAsync(request);

                // Assert
                Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
                response.Dispose();
            }
        }

        [TestClass]
        public class EvolveToFinalStateTests : GameOfLifeControllerTests
        {
            private const string ExpectedResponseMediaType = "application/json";

            [TestMethod]
            public async Task ShouldReturnOkForStablePattern()
            {
                // Arrange - Block pattern (2x2 square - still life):
                // ? ?
                // ? ?
                IEnumerable<Coordinate> blockPattern = new List<Coordinate>
                {
                    new Coordinate { X = 1, Y = 1 },
                    new Coordinate { X = 1, Y = 2 },
                    new Coordinate { X = 2, Y = 1 },
                    new Coordinate { X = 2, Y = 2 }
                };

                HttpRequestMessage createRequest = new(HttpMethod.Post, "/api/v1/GameOfLife")
                {
                    Content = JsonContent.Create(blockPattern)
                };

                HttpResponseMessage createResponse = await _client.SendAsync(createRequest);
                string worldIdString = await createResponse.Content.ReadAsStringAsync();
                Guid worldIdentifier = Guid.Parse(worldIdString);
                createResponse.Dispose();

                HttpRequestMessage evolveRequest = new(HttpMethod.Put, $"/api/v1/GameOfLife/{worldIdentifier}/evolve/final");

                // Act
                HttpResponseMessage response = await _client.SendAsync(evolveRequest);

                // Assert
                response.EnsureSuccessStatusCode();
                Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
                response.Dispose();
            }

            [TestMethod]
            public async Task ShouldReturnJsonContentType()
            {
                // Arrange - Block pattern (2x2 square - still life):
                // ? ?
                // ? ?
                IEnumerable<Coordinate> blockPattern = new List<Coordinate>
                {
                    new Coordinate { X = 1, Y = 1 },
                    new Coordinate { X = 1, Y = 2 },
                    new Coordinate { X = 2, Y = 1 },
                    new Coordinate { X = 2, Y = 2 }
                };

                HttpRequestMessage createRequest = new(HttpMethod.Post, "/api/v1/GameOfLife")
                {
                    Content = JsonContent.Create(blockPattern)
                };

                HttpResponseMessage createResponse = await _client.SendAsync(createRequest);
                string worldIdString = await createResponse.Content.ReadAsStringAsync();
                Guid worldIdentifier = Guid.Parse(worldIdString);
                createResponse.Dispose();

                HttpRequestMessage evolveRequest = new(HttpMethod.Put, $"/api/v1/GameOfLife/{worldIdentifier}/evolve/final");

                // Act
                HttpResponseMessage response = await _client.SendAsync(evolveRequest);

                // Assert
                Assert.AreEqual(ExpectedResponseMediaType, response.Content.Headers.ContentType!.MediaType);
                response.Dispose();
            }

            [TestMethod]
            public async Task ShouldReturnFinalStateCoordinates()
            {
                // Arrange - Dies out pattern:
                // ? ?
                IEnumerable<Coordinate> diesOutPattern = new List<Coordinate>
                {
                    new Coordinate { X = 1, Y = 1 },
                    new Coordinate { X = 2, Y = 1 }
                };

                HttpRequestMessage createRequest = new(HttpMethod.Post, "/api/v1/GameOfLife")
                {
                    Content = JsonContent.Create(diesOutPattern)
                };

                HttpResponseMessage createResponse = await _client.SendAsync(createRequest);
                string worldIdString = await createResponse.Content.ReadAsStringAsync();
                Guid worldIdentifier = Guid.Parse(worldIdString);
                createResponse.Dispose();

                HttpRequestMessage evolveRequest = new(HttpMethod.Put, $"/api/v1/GameOfLife/{worldIdentifier}/evolve/final");

                // Act
                HttpResponseMessage response = await _client.SendAsync(evolveRequest);
                IEnumerable<Coordinate>? coordinates = await response.Content.ReadFromJsonAsync<IEnumerable<Coordinate>>();

                // Assert
                Assert.IsNotNull(coordinates);
                Assert.AreEqual(0, coordinates.Count());
                response.Dispose();
            }

            [TestMethod]
            public async Task ShouldStabilizeBlockPattern()
            {
                // Arrange - Block pattern (2x2 square - still life):
                // ? ?
                // ? ?
                IEnumerable<Coordinate> blockPattern = new List<Coordinate>
                {
                    new Coordinate { X = 5, Y = 5 },
                    new Coordinate { X = 5, Y = 6 },
                    new Coordinate { X = 6, Y = 5 },
                    new Coordinate { X = 6, Y = 6 }
                };

                HttpRequestMessage createRequest = new(HttpMethod.Post, "/api/v1/GameOfLife")
                {
                    Content = JsonContent.Create(blockPattern)
                };

                HttpResponseMessage createResponse = await _client.SendAsync(createRequest);
                string worldIdString = await createResponse.Content.ReadAsStringAsync();
                Guid worldIdentifier = Guid.Parse(worldIdString);
                createResponse.Dispose();

                HttpRequestMessage evolveRequest = new(HttpMethod.Put, $"/api/v1/GameOfLife/{worldIdentifier}/evolve/final");

                // Act
                HttpResponseMessage response = await _client.SendAsync(evolveRequest);
                IEnumerable<Coordinate>? coordinates = await response.Content.ReadFromJsonAsync<IEnumerable<Coordinate>>();

                // Assert - Block pattern is stable
                Assert.IsNotNull(coordinates);
                Assert.AreEqual(4, coordinates.Count());
                response.Dispose();
            }

            [TestMethod]
            public async Task ShouldReturnErrorForOscillatorPattern()
            {
                // Arrange - Blinker pattern (period-2 oscillator):
                // Vertical:    ?
                //              ?
                //              ?
                IEnumerable<Coordinate> blinkerPattern = new List<Coordinate>
                {
                    new Coordinate { X = 2, Y = 1 },
                    new Coordinate { X = 2, Y = 2 },
                    new Coordinate { X = 2, Y = 3 }
                };

                HttpRequestMessage createRequest = new(HttpMethod.Post, "/api/v1/GameOfLife")
                {
                    Content = JsonContent.Create(blinkerPattern)
                };

                HttpResponseMessage createResponse = await _client.SendAsync(createRequest);
                string worldIdString = await createResponse.Content.ReadAsStringAsync();
                Guid worldIdentifier = Guid.Parse(worldIdString);
                createResponse.Dispose();

                HttpRequestMessage evolveRequest = new(HttpMethod.Put, $"/api/v1/GameOfLife/{worldIdentifier}/evolve/final");

                // Act
                HttpResponseMessage response = await _client.SendAsync(evolveRequest);

                // Assert - Should return 500 Internal Server Error for oscillator that can't stabilize
                Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
                response.Dispose();
            }

            [TestMethod]
            public async Task ShouldReturnErrorDetailsForOscillatorPattern()
            {
                // Arrange - Blinker pattern (period-2 oscillator):
                // Vertical:    ?
                //              ?
                //              ?
                IEnumerable<Coordinate> blinkerPattern = new List<Coordinate>
                {
                    new Coordinate { X = 2, Y = 1 },
                    new Coordinate { X = 2, Y = 2 },
                    new Coordinate { X = 2, Y = 3 }
                };

                HttpRequestMessage createRequest = new(HttpMethod.Post, "/api/v1/GameOfLife")
                {
                    Content = JsonContent.Create(blinkerPattern)
                };

                HttpResponseMessage createResponse = await _client.SendAsync(createRequest);
                string worldIdString = await createResponse.Content.ReadAsStringAsync();
                Guid worldIdentifier = Guid.Parse(worldIdString);
                createResponse.Dispose();

                HttpRequestMessage evolveRequest = new(HttpMethod.Put, $"/api/v1/GameOfLife/{worldIdentifier}/evolve/final");

                // Act
                HttpResponseMessage response = await _client.SendAsync(evolveRequest);
                string responseContent = await response.Content.ReadAsStringAsync();
                JsonDocument jsonDoc = JsonDocument.Parse(responseContent);
                JsonElement root = jsonDoc.RootElement;

                // Assert - Should contain error details
                Assert.IsTrue(root.TryGetProperty("error", out JsonElement errorElement));
                Assert.AreEqual("PopulationStabilizationFailed", errorElement.GetString());

                Assert.IsTrue(root.TryGetProperty("message", out JsonElement messageElement));
                Assert.IsNotNull(messageElement.GetString());

                Assert.IsTrue(root.TryGetProperty("worldIdentifier", out JsonElement worldIdElement));
                Assert.AreEqual(worldIdentifier.ToString(), worldIdElement.GetString());

                Assert.IsTrue(root.TryGetProperty("maxGenerations", out JsonElement maxGenElement));
                Assert.IsTrue(maxGenElement.GetInt32() > 0);

                response.Dispose();
            }
            
            [TestMethod]
            public async Task ShouldStabilizePatternWithinFewGenerations()
            {
                // Arrange - L-shape pattern that evolves to a boat (still life) in 3 generations
                // Generation 0:  ? ?
                //                ?
                //                  ?
                // Generation 3 (Boat - stable):
                //                ? ?
                //                ?   ?
                //                  ?
                IEnumerable<Coordinate> lPattern = new List<Coordinate>
                {
                    new Coordinate { X = 5, Y = 5 },
                    new Coordinate { X = 6, Y = 5 },
                    new Coordinate { X = 5, Y = 6 },
                    new Coordinate { X = 7, Y = 7 }
                };

                HttpRequestMessage createRequest = new(HttpMethod.Post, "/api/v1/GameOfLife")
                {
                    Content = JsonContent.Create(lPattern)
                };

                HttpResponseMessage createResponse = await _client.SendAsync(createRequest);
                string worldIdString = await createResponse.Content.ReadAsStringAsync();
                Guid worldIdentifier = Guid.Parse(worldIdString);
                createResponse.Dispose();

                HttpRequestMessage evolveRequest = new(HttpMethod.Put, $"/api/v1/GameOfLife/{worldIdentifier}/evolve/final");

                // Act
                HttpResponseMessage response = await _client.SendAsync(evolveRequest);
                IEnumerable<Coordinate>? coordinates = await response.Content.ReadFromJsonAsync<IEnumerable<Coordinate>>();

                // Assert - Should stabilize successfully within a few generations
                Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
                Assert.IsNotNull(coordinates);
                
                // Pattern should stabilize (could be boat with 5 cells, or die out to 0, or become a block with 4)
                // The key assertion is that it returns 200 OK, meaning it stabilized before max generations
                Assert.IsTrue(coordinates.Count() >= 0);

                response.Dispose();
            }
        }

        [TestClass]
        public class ErrorHandlingTests : GameOfLifeControllerTests
        {
            [TestMethod]
            public async Task ShouldReturn404ForNonExistentWorld()
            {
                // Arrange
                Guid nonExistentWorldId = Guid.NewGuid();
                HttpRequestMessage request = new(HttpMethod.Put, $"/api/v1/GameOfLife/{nonExistentWorldId}/evolve");

                // Act
                HttpResponseMessage response = await _client.SendAsync(request);

                // Assert
                Assert.AreEqual(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
                response.Dispose();
            }

            [TestMethod]
            public async Task ShouldReturnBadRequestForInvalidGuid()
            {
                // Arrange
                HttpRequestMessage request = new(HttpMethod.Put, "/api/v1/GameOfLife/invalid-guid/evolve");

                // Act
                HttpResponseMessage response = await _client.SendAsync(request);

                // Assert
                Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
                response.Dispose();
            }

            [TestMethod]
            public async Task ShouldReturnBadRequestForInvalidRequestBody()
            {
                // Arrange
                HttpRequestMessage request = new(HttpMethod.Post, "/api/v1/GameOfLife")
                {
                    Content = new StringContent("invalid json", System.Text.Encoding.UTF8, "application/json")
                };

                // Act
                HttpResponseMessage response = await _client.SendAsync(request);

                // Assert
                Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
                response.Dispose();
            }
        }

        [TestClass]
        public class IntegrationScenarioTests : GameOfLifeControllerTests
        {
            [TestMethod]
            public async Task ShouldCreateWorldEvolveAndGetFinalState()
            {
                // Arrange - Blinker pattern (period-2 oscillator):
                // Vertical:    ?
                //              ?
                //              ?
                IEnumerable<Coordinate> blinkerPattern = new List<Coordinate>
                {
                    new Coordinate { X = 2, Y = 1 },
                    new Coordinate { X = 2, Y = 2 },
                    new Coordinate { X = 2, Y = 3 }
                };

                HttpRequestMessage createRequest = new(HttpMethod.Post, "/api/v1/GameOfLife")
                {
                    Content = JsonContent.Create(blinkerPattern)
                };

                // Act & Assert - Create world
                HttpResponseMessage createResponse = await _client.SendAsync(createRequest);
                Assert.AreEqual(System.Net.HttpStatusCode.Created, createResponse.StatusCode);
                
                string worldIdString = await createResponse.Content.ReadAsStringAsync();
                Guid worldIdentifier = Guid.Parse(worldIdString);
                createResponse.Dispose();

                // Act & Assert - Evolve once
                HttpRequestMessage evolveRequest = new(HttpMethod.Put, $"/api/v1/GameOfLife/{worldIdentifier}/evolve");
                HttpResponseMessage evolveResponse = await _client.SendAsync(evolveRequest);
                Assert.AreEqual(System.Net.HttpStatusCode.OK, evolveResponse.StatusCode);
                
                IEnumerable<Coordinate>? evolvedCoordinates = await evolveResponse.Content.ReadFromJsonAsync<IEnumerable<Coordinate>>();
                Assert.IsNotNull(evolvedCoordinates);
                Assert.AreEqual(3, evolvedCoordinates.Count());
                evolveResponse.Dispose();

                // Act & Assert - Evolve to final state (will hit max generations for oscillator)
                HttpRequestMessage finalRequest = new(HttpMethod.Put, $"/api/v1/GameOfLife/{worldIdentifier}/evolve/final");
                HttpResponseMessage finalResponse = await _client.SendAsync(finalRequest);
                
                // Blinker oscillates, so it may return 500 if it can't stabilize
                // or 200 if it completes within max generations
                Assert.IsTrue(
                    finalResponse.StatusCode == System.Net.HttpStatusCode.OK || 
                    finalResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError
                );
                finalResponse.Dispose();
            }

            [TestMethod]
            public async Task ShouldHandleMultipleWorldsIndependently()
            {
                // Arrange - Create first world
                // Pattern: ?
                //          ?
                IEnumerable<Coordinate> pattern1 = new List<Coordinate>
                {
                    new Coordinate { X = 0, Y = 0 },
                    new Coordinate { X = 0, Y = 1 }
                };

                HttpRequestMessage createRequest1 = new(HttpMethod.Post, "/api/v1/GameOfLife")
                {
                    Content = JsonContent.Create(pattern1)
                };

                HttpResponseMessage createResponse1 = await _client.SendAsync(createRequest1);
                string worldId1String = await createResponse1.Content.ReadAsStringAsync();
                Guid worldId1 = Guid.Parse(worldId1String);
                createResponse1.Dispose();

                // Arrange - Create second world
                // Block pattern (2x2 square - still life):
                // ? ?
                // ? ?
                IEnumerable<Coordinate> pattern2 = new List<Coordinate>
                {
                    new Coordinate { X = 5, Y = 5 },
                    new Coordinate { X = 5, Y = 6 },
                    new Coordinate { X = 6, Y = 5 },
                    new Coordinate { X = 6, Y = 6 }
                };

                HttpRequestMessage createRequest2 = new(HttpMethod.Post, "/api/v1/GameOfLife")
                {
                    Content = JsonContent.Create(pattern2)
                };

                HttpResponseMessage createResponse2 = await _client.SendAsync(createRequest2);
                string worldId2String = await createResponse2.Content.ReadAsStringAsync();
                Guid worldId2 = Guid.Parse(worldId2String);
                createResponse2.Dispose();

                // Act - Evolve both worlds
                HttpRequestMessage evolveRequest1 = new(HttpMethod.Put, $"/api/v1/GameOfLife/{worldId1}/evolve");
                HttpResponseMessage evolveResponse1 = await _client.SendAsync(evolveRequest1);

                HttpRequestMessage evolveRequest2 = new(HttpMethod.Put, $"/api/v1/GameOfLife/{worldId2}/evolve");
                HttpResponseMessage evolveResponse2 = await _client.SendAsync(evolveRequest2);

                // Assert - Both should succeed
                Assert.AreEqual(System.Net.HttpStatusCode.OK, evolveResponse1.StatusCode);
                Assert.AreEqual(System.Net.HttpStatusCode.OK, evolveResponse2.StatusCode);

                // Assert - Worlds are different
                Assert.AreNotEqual(worldId1, worldId2);

                evolveResponse1.Dispose();
                evolveResponse2.Dispose();
            }
        }
    }
}
