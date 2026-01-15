using GameOfLife.API.Controllers;
using GameOfLife.API.Exceptions;
using GameOfLife.API.Models;
using GameOfLife.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GameOfLife.API.UnitTests.Controllers
{
    [TestClass]
    public abstract class GameOfLifeControllerTests
    {
        protected GameOfLifeController _controller;
        protected Mock<IGameOfLifeService> _mockGameOfLifeService;

        [TestInitialize]
        public void Initialize()
        {
            _mockGameOfLifeService = new Mock<IGameOfLifeService>();
            _controller = new GameOfLifeController(_mockGameOfLifeService.Object);
        }

        [TestClass]
        public class CreateWorldTests : GameOfLifeControllerTests
        {
            private IEnumerable<Coordinate> _request;

            [TestInitialize]
            public void Setup()
            {
                _mockGameOfLifeService
                    .Setup(s => s.CreateWorldAsync(It.IsAny<IEnumerable<Coordinate>>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Guid.NewGuid());

                _request = [
                            new Coordinate(0, 0),
                            new Coordinate(1, 1)
                          ];
            }

            [TestMethod]
            public async Task ShouldReturnCreatedStatusCode()
            {
                // Act
                var result = await _controller.CreateWorld(_request, CancellationToken.None);

                // Assert
                Assert.IsInstanceOfType(result, typeof(ObjectResult));
                var objectResult = (ObjectResult)result;
                Assert.AreEqual(StatusCodes.Status201Created, objectResult.StatusCode);
            }

            [TestMethod]
            public async Task ShouldReturnWorldIdentifier()
            {
                // Arrange
                Guid expectedWorldId = Guid.NewGuid();
                _mockGameOfLifeService
                    .Setup(s => s.CreateWorldAsync(It.IsAny<IEnumerable<Coordinate>>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(expectedWorldId);


                // Act
                var result = await _controller.CreateWorld(_request, CancellationToken.None);

                // Assert
                var objectResult = (ObjectResult)result;
                Assert.AreEqual(expectedWorldId.ToString(), objectResult.Value);
            }

            [TestMethod]
            public async Task ShouldCallService()
            {
                // Act
                await _controller.CreateWorld(_request, CancellationToken.None);

                // Assert
                _mockGameOfLifeService.Verify(s => s.CreateWorldAsync(It.IsAny<IEnumerable<Coordinate>>(), It.IsAny<CancellationToken>()), Times.Once);
            }

            [TestMethod]
            public async Task ShouldPassExactCoordinatesToService()
            {
                // Arrange
                IEnumerable<Coordinate>? capturedCoordinates = null;
                _mockGameOfLifeService
                    .Setup(s => s.CreateWorldAsync(It.IsAny<IEnumerable<Coordinate>>(), It.IsAny<CancellationToken>()))
                    .Callback<IEnumerable<Coordinate>, CancellationToken>((coords, _) => capturedCoordinates = coords)
                    .ReturnsAsync(Guid.NewGuid());

                // Act
                await _controller.CreateWorld(_request, CancellationToken.None);

                // Assert
                Assert.IsNotNull(capturedCoordinates);
                Assert.IsTrue(_request.SequenceEqual(capturedCoordinates));
            }

            [TestMethod]
            public async Task ShouldPassCancellationTokenToService()
            {
                // Arrange
                var expectedCancellationToken = new CancellationToken();
                CancellationToken capturedCancellationToken = default;
                _mockGameOfLifeService
                    .Setup(s => s.CreateWorldAsync(It.IsAny<IEnumerable<Coordinate>>(), It.IsAny<CancellationToken>()))
                    .Callback<IEnumerable<Coordinate>, CancellationToken>((_, ct) => capturedCancellationToken = ct)
                    .ReturnsAsync(Guid.NewGuid());

                // Act
                await _controller.CreateWorld(_request, expectedCancellationToken);

                // Assert
                Assert.AreEqual(expectedCancellationToken, capturedCancellationToken);
            }
        }

        [TestClass]
        public class EvolveTests : GameOfLifeControllerTests
        {
            private Guid _requestWorldIdentifier;
            private IEnumerable<Coordinate> _expectedCoordinates;

            [TestInitialize]
            public void Setup()
            {
                _requestWorldIdentifier = Guid.NewGuid();

                _expectedCoordinates = [
                                            new Coordinate(1, 1),
                                            new Coordinate(2, 2)
                                       ];
                _mockGameOfLifeService
                    .Setup(s => s.EvolveWorldAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(_expectedCoordinates);
            }

            [TestMethod]
            public async Task ShouldReturnOkStatusCode()
            {
                // Act
                var result = await _controller.Evolve(_requestWorldIdentifier);

                // Assert
                Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            }

            [TestMethod]
            public async Task ShouldReturnEvolvedCoordinates()
            {
                // Act
                var result = await _controller.Evolve(_requestWorldIdentifier);

                // Assert
                var okResult = (OkObjectResult)result;
                Assert.AreEqual(_expectedCoordinates, okResult.Value);
            }

            [TestMethod]
            public async Task ShouldCallService()
            {
                // Act
                await _controller.Evolve(_requestWorldIdentifier);

                // Assert
                _mockGameOfLifeService.Verify(m => m.EvolveWorldAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            }

            [TestMethod]
            public async Task ShouldPassExactWorldIdentifierToService()
            {
                // Arrange
                Guid capturedWorldId = Guid.Empty;
                _mockGameOfLifeService
                    .Setup(s => s.EvolveWorldAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .Callback<Guid, int, CancellationToken>((worldId, _, _) => capturedWorldId = worldId)
                    .ReturnsAsync(_expectedCoordinates);

                // Act
                await _controller.Evolve(_requestWorldIdentifier);

                // Assert
                Assert.AreEqual(_requestWorldIdentifier, capturedWorldId);
            }

            [TestMethod]
            public async Task ShouldCallServiceWithDefaultGenerationCount()
            {
                // Arrange
                int capturedGenerationCount = 0;
                _mockGameOfLifeService
                    .Setup(s => s.EvolveWorldAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .Callback<Guid, int, CancellationToken>((_, genCount, _) => capturedGenerationCount = genCount)
                    .ReturnsAsync(_expectedCoordinates);

                // Act
                await _controller.Evolve(_requestWorldIdentifier);

                // Assert
                Assert.AreEqual(1, capturedGenerationCount);
            }

            [TestMethod]
            [DataRow(1)]
            [DataRow(5)]
            [DataRow(10)]
            [DataRow(100)]
            [DataRow(1000)]
            public async Task ShouldCallServiceWithSpecifiedGenerationCount(int expectedGenerationCount)
            {
                // Arrange
                int capturedGenerationCount = 0;
                _mockGameOfLifeService
                    .Setup(s => s.EvolveWorldAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .Callback<Guid, int, CancellationToken>((_, genCount, _) => capturedGenerationCount = genCount)
                    .ReturnsAsync(_expectedCoordinates);

                // Act
                await _controller.Evolve(_requestWorldIdentifier, expectedGenerationCount);

                // Assert
                Assert.AreEqual(expectedGenerationCount, capturedGenerationCount);
            }

            [TestMethod]
            public async Task ShouldPassCancellationTokenToService()
            {
                // Arrange
                var expectedCancellationToken = new CancellationToken();
                CancellationToken capturedCancellationToken = default;
                _mockGameOfLifeService
                    .Setup(s => s.EvolveWorldAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .Callback<Guid, int, CancellationToken>((_, _, ct) => capturedCancellationToken = ct)
                    .ReturnsAsync(_expectedCoordinates);

                // Act
                await _controller.Evolve(_requestWorldIdentifier, 1, expectedCancellationToken);

                // Assert
                Assert.AreEqual(expectedCancellationToken, capturedCancellationToken);
            }
        }

        [TestClass]
        public class EvolveToFinalStateTests : GameOfLifeControllerTests
        {
            private Guid _requestWorldIdentifier;
            private IEnumerable<Coordinate> _expectedCoordinates;

            [TestInitialize]
            public void Setup()
            {
                _requestWorldIdentifier = Guid.NewGuid();

                _expectedCoordinates = [
                                            new Coordinate(1, 1),
                                            new Coordinate(2, 2)
                                       ];
                _mockGameOfLifeService
                    .Setup(s => s.EvolveToFinalWorldStateAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(_expectedCoordinates);
            }

            [TestMethod]
            public async Task ShouldReturnOkStatusCodeWhenSuccessful()
            {
                // Act
                var result = await _controller.EvolveToFinalState(_requestWorldIdentifier, CancellationToken.None);

                // Assert
                Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            }

            [TestMethod]
            public async Task ShouldReturnFinalStateCoordinates()
            {
                // Act
                var result = await _controller.EvolveToFinalState(_requestWorldIdentifier, CancellationToken.None);

                // Assert
                var okResult = (OkObjectResult)result;
                Assert.AreEqual(_expectedCoordinates, okResult.Value);
            }

            [TestMethod]
            public async Task ShouldReturn500WhenUnableToStabilize()
            {
                // Arrange
                var maxGenerations = 1000;
                var exception = new UnableToStabilizePopulationException(maxGenerations);
                _mockGameOfLifeService
                    .Setup(s => s.EvolveToFinalWorldStateAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(exception);

                // Act
                var result = await _controller.EvolveToFinalState(_requestWorldIdentifier, CancellationToken.None);

                // Assert
                Assert.IsInstanceOfType(result, typeof(ObjectResult));
                var objectResult = (ObjectResult)result;
                Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            }

            [TestMethod]
            public async Task ShouldReturnErrorDetailsWhenUnableToStabilize()
            {
                // Arrange
                var maxGenerations = 1000;
                var exception = new UnableToStabilizePopulationException(maxGenerations);
                _mockGameOfLifeService
                    .Setup(s => s.EvolveToFinalWorldStateAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(exception);

                // Act
                var result = await _controller.EvolveToFinalState(_requestWorldIdentifier, CancellationToken.None);

                // Assert
                var objectResult = (ObjectResult)result;
                var errorResponse = objectResult.Value;
                
                Assert.IsNotNull(errorResponse);
                var errorType = errorResponse.GetType();
                var errorProperty = errorType.GetProperty("error");
                var messageProperty = errorType.GetProperty("message");
                var worldIdentifierProperty = errorType.GetProperty("worldIdentifier");
                var maxGenerationsProperty = errorType.GetProperty("maxGenerations");

                Assert.AreEqual("PopulationStabilizationFailed", errorProperty?.GetValue(errorResponse));
                Assert.AreEqual(exception.Message, messageProperty?.GetValue(errorResponse));
                Assert.AreEqual(_requestWorldIdentifier.ToString(), worldIdentifierProperty?.GetValue(errorResponse));
                Assert.AreEqual(maxGenerations, maxGenerationsProperty?.GetValue(errorResponse));
            }

            [TestMethod]
            public async Task ShouldCallService()
            {
                // Arrange
                Guid capturedWorldId = Guid.Empty;
                _mockGameOfLifeService
                    .Setup(s => s.EvolveToFinalWorldStateAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .Callback<Guid, CancellationToken>((worldId, _) => capturedWorldId = worldId)
                    .ReturnsAsync(_expectedCoordinates);

                // Act
                await _controller.EvolveToFinalState(_requestWorldIdentifier, CancellationToken.None);

                // Assert
                _mockGameOfLifeService.Verify(m => m.EvolveToFinalWorldStateAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            }

            [TestMethod]
            public async Task ShouldPassExactWorldIdentifierToService()
            {
                // Arrange
                Guid capturedWorldId = Guid.Empty;
                _mockGameOfLifeService
                    .Setup(s => s.EvolveToFinalWorldStateAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .Callback<Guid, CancellationToken>((worldId, _) => capturedWorldId = worldId)
                    .ReturnsAsync(_expectedCoordinates);

                // Act
                await _controller.EvolveToFinalState(_requestWorldIdentifier, CancellationToken.None);

                // Assert
                Assert.AreEqual(_requestWorldIdentifier, capturedWorldId);
            }

            [TestMethod]
            public async Task ShouldPassCancellationTokenToService()
            {
                // Arrange
                var expectedCancellationToken = new CancellationToken();
                CancellationToken capturedCancellationToken = default;
                _mockGameOfLifeService
                    .Setup(s => s.EvolveToFinalWorldStateAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .Callback<Guid, CancellationToken>((_, ct) => capturedCancellationToken = ct)
                    .ReturnsAsync(_expectedCoordinates);

                // Act
                await _controller.EvolveToFinalState(_requestWorldIdentifier, expectedCancellationToken);

                // Assert
                Assert.AreEqual(expectedCancellationToken, capturedCancellationToken);
            }
        }
    }
}
