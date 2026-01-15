using GameOfLife.API.Models;
using GameOfLife.API.Services;
using GameOfLife.API.Services.Metrics;
using GameOfLife.API.Services.Rules;
using Moq;

namespace GameOfLife.API.UnitTests.Services
{
    [TestClass]
    /// NOTE: These tests should be optimized by moving the common setups into test initialize.
    public abstract class WorldTests
    {
        protected World _world;

        protected Mock<IEvolutionRules> _mockEvolutionRules;
        protected Mock<IMetricPublisher> _mockMetricPublisher;

        [TestInitialize]
        public void Initialize()
        {
            _mockEvolutionRules = new Mock<IEvolutionRules>();
            _mockMetricPublisher = new Mock<IMetricPublisher>();

            _world = new World(_mockEvolutionRules.Object, _mockMetricPublisher.Object);
        }

        [TestClass]
        public class InitializeAsyncTests : WorldTests
        {
            [TestMethod]
            public async Task ShouldInitializeWithProvidedEntities()
            {
                // Arrange
                IEnumerable<Entity> entities = new List<Entity>
                {
                    new Entity(0, 0),
                    new Entity(1, 1),
                    new Entity(2, 2)
                };

                // Act
                await _world.InitializeAsync(entities);
                IEnumerable<Entity> livingEntities = _world.GetLivingEntities();

                // Assert
                Assert.AreEqual(3, livingEntities.Count());
                Assert.IsTrue(livingEntities.Contains(new Entity(0, 0)));
                Assert.IsTrue(livingEntities.Contains(new Entity(1, 1)));
                Assert.IsTrue(livingEntities.Contains(new Entity(2, 2)));
            }

            [TestMethod]
            public async Task ShouldInitializeWithEmptyCollection()
            {
                // Arrange
                IEnumerable<Entity> entities = new List<Entity>();

                // Act
                await _world.InitializeAsync(entities);
                IEnumerable<Entity> livingEntities = _world.GetLivingEntities();

                // Assert
                Assert.AreEqual(0, livingEntities.Count());
            }

            [TestMethod]
            public async Task ShouldReplaceExistingEntitiesOnReinitialization()
            {
                // Arrange
                IEnumerable<Entity> initialEntities = new List<Entity>
                {
                    new Entity(0, 0),
                    new Entity(1, 1)
                };
                IEnumerable<Entity> newEntities = new List<Entity>
                {
                    new Entity(5, 5),
                    new Entity(6, 6)
                };

                // Act
                await _world.InitializeAsync(initialEntities);
                await _world.InitializeAsync(newEntities);
                IEnumerable<Entity> livingEntities = _world.GetLivingEntities();

                // Assert
                Assert.AreEqual(2, livingEntities.Count());
                Assert.IsTrue(livingEntities.Contains(new Entity(5, 5)));
                Assert.IsTrue(livingEntities.Contains(new Entity(6, 6)));
                Assert.IsFalse(livingEntities.Contains(new Entity(0, 0)));
                Assert.IsFalse(livingEntities.Contains(new Entity(1, 1)));
            }
        }

        [TestClass]
        public class GetLivingEntitiesTests : WorldTests
        {
            [TestMethod]
            public async Task ShouldReturnEmptyCollectionWhenNotInitialized()
            {
                // Act
                IEnumerable<Entity> livingEntities = _world.GetLivingEntities();

                // Assert
                Assert.IsNotNull(livingEntities);
                Assert.AreEqual(0, livingEntities.Count());
            }

            [TestMethod]
            public async Task ShouldReturnCopyOfLivingEntities()
            {
                // Arrange
                IEnumerable<Entity> entities = new List<Entity>
                {
                    new Entity(0, 0),
                    new Entity(1, 1)
                };
                await _world.InitializeAsync(entities);

                // Act
                IEnumerable<Entity> livingEntities1 = _world.GetLivingEntities();
                IEnumerable<Entity> livingEntities2 = _world.GetLivingEntities();

                // Assert
                Assert.AreNotSame(livingEntities1, livingEntities2);
                Assert.IsTrue(livingEntities1.SequenceEqual(livingEntities2));
            }
        }

        [TestClass]
        public class EvolveAsyncTests : WorldTests
        {
            [TestInitialize]
            public void SetupMocks()
            {
                _mockMetricPublisher
                    .Setup(m => m.PublishBirthCount(It.IsAny<int>()))
                    .Returns(Task.CompletedTask);
                _mockMetricPublisher
                    .Setup(m => m.PublishDeathCount(It.IsAny<int>()))
                    .Returns(Task.CompletedTask);
                _mockMetricPublisher
                    .Setup(m => m.PublishPopulationCount(It.IsAny<int>()))
                    .Returns(Task.CompletedTask);
            }

            [TestMethod]
            public async Task ShouldReturnFalseWhenNoEvolutionOccurs()
            {
                // Arrange
                Mock<IProximityRule> mockStayAliveRule = new Mock<IProximityRule>();
                mockStayAliveRule.Setup(r => r.HasSufficientNeighbors(It.IsAny<byte>())).Returns(true);
                
                _mockEvolutionRules.Setup(r => r.StayAliveRules).Returns(new[] { mockStayAliveRule.Object });
                _mockEvolutionRules.Setup(r => r.BirthRules).Returns(Array.Empty<IProximityRule>());

                IEnumerable<Entity> entities = new List<Entity>
                {
                    new Entity(1, 1),
                    new Entity(1, 2),
                    new Entity(2, 1)
                };
                await _world.InitializeAsync(entities);

                // Act
                bool evolved = await _world.EvolveAsync();

                // Assert
                Assert.IsFalse(evolved);
            }

            [TestMethod]
            public async Task ShouldReturnTrueWhenEvolutionOccurs()
            {
                // Arrange
                Mock<IProximityRule> mockStayAliveRule = new Mock<IProximityRule>();
                mockStayAliveRule.Setup(r => r.HasSufficientNeighbors(1)).Returns(false);
                mockStayAliveRule.Setup(r => r.HasSufficientNeighbors(2)).Returns(true);
                
                _mockEvolutionRules.Setup(r => r.StayAliveRules).Returns(new[] { mockStayAliveRule.Object });
                _mockEvolutionRules.Setup(r => r.BirthRules).Returns(Array.Empty<IProximityRule>());

                IEnumerable<Entity> entities = new List<Entity>
                {
                    new Entity(0, 0),
                    new Entity(1, 0),
                    new Entity(2, 0)
                };
                await _world.InitializeAsync(entities);

                // Act
                bool evolved = await _world.EvolveAsync();

                // Assert
                Assert.IsTrue(evolved);
            }

            [TestMethod]
            public async Task ShouldKillEntitiesNotMeetingStayAliveRules()
            {
                // Arrange
                Mock<IProximityRule> mockStayAliveRule = new Mock<IProximityRule>();
                mockStayAliveRule.Setup(r => r.HasSufficientNeighbors(It.IsAny<byte>())).Returns(false);
                
                _mockEvolutionRules.Setup(r => r.StayAliveRules).Returns(new[] { mockStayAliveRule.Object });
                _mockEvolutionRules.Setup(r => r.BirthRules).Returns(Array.Empty<IProximityRule>());

                IEnumerable<Entity> entities = new List<Entity>
                {
                    new Entity(1, 1),
                    new Entity(5, 5)
                };
                await _world.InitializeAsync(entities);

                // Act
                await _world.EvolveAsync();
                IEnumerable<Entity> livingEntities = _world.GetLivingEntities();

                // Assert
                Assert.AreEqual(0, livingEntities.Count());
            }

            [TestMethod]
            public async Task ShouldKeepEntitiesMeetingStayAliveRules()
            {
                // Arrange
                Mock<IProximityRule> mockStayAliveRule = new Mock<IProximityRule>();
                mockStayAliveRule.Setup(r => r.HasSufficientNeighbors(2)).Returns(true);
                mockStayAliveRule.Setup(r => r.HasSufficientNeighbors(3)).Returns(true);
                
                _mockEvolutionRules.Setup(r => r.StayAliveRules).Returns(new[] { mockStayAliveRule.Object });
                _mockEvolutionRules.Setup(r => r.BirthRules).Returns(Array.Empty<IProximityRule>());

                IEnumerable<Entity> entities = new List<Entity>
                {
                    new Entity(1, 1),
                    new Entity(1, 2),
                    new Entity(2, 1)
                };
                await _world.InitializeAsync(entities);

                // Act
                await _world.EvolveAsync();
                IEnumerable<Entity> livingEntities = _world.GetLivingEntities();

                // Assert
                Assert.AreEqual(3, livingEntities.Count());
                Assert.IsTrue(livingEntities.Contains(new Entity(1, 1)));
                Assert.IsTrue(livingEntities.Contains(new Entity(1, 2)));
                Assert.IsTrue(livingEntities.Contains(new Entity(2, 1)));
            }

            [TestMethod]
            public async Task ShouldBirthEntitiesMeetingBirthRules()
            {
                // Arrange
                Mock<IProximityRule> mockStayAliveRule = new Mock<IProximityRule>();
                mockStayAliveRule.Setup(r => r.HasSufficientNeighbors(It.IsAny<byte>())).Returns(true);
                
                Mock<IProximityRule> mockBirthRule = new Mock<IProximityRule>();
                mockBirthRule.Setup(r => r.HasSufficientNeighbors(3)).Returns(true);
                
                _mockEvolutionRules.Setup(r => r.StayAliveRules).Returns(new[] { mockStayAliveRule.Object });
                _mockEvolutionRules.Setup(r => r.BirthRules).Returns(new[] { mockBirthRule.Object });

                IEnumerable<Entity> entities = new List<Entity>
                {
                    new Entity(0, 0),
                    new Entity(1, 0),
                    new Entity(0, 1)
                };
                await _world.InitializeAsync(entities);

                // Act
                await _world.EvolveAsync();
                IEnumerable<Entity> livingEntities = _world.GetLivingEntities();

                // Assert
                Assert.IsTrue(livingEntities.Count() >= 3);
                Assert.IsTrue(livingEntities.Contains(new Entity(1, 1)));
            }

            [TestMethod]
            public async Task ShouldPublishMetricsAfterEvolution()
            {
                // Arrange
                Mock<IProximityRule> mockStayAliveRule = new Mock<IProximityRule>();
                mockStayAliveRule.Setup(r => r.HasSufficientNeighbors(It.IsAny<byte>())).Returns(false);
                
                _mockEvolutionRules.Setup(r => r.StayAliveRules).Returns(new[] { mockStayAliveRule.Object });
                _mockEvolutionRules.Setup(r => r.BirthRules).Returns(Array.Empty<IProximityRule>());

                IEnumerable<Entity> entities = new List<Entity> { new Entity(1, 1) };
                await _world.InitializeAsync(entities);

                // Act
                await _world.EvolveAsync();

                // Assert
                _mockMetricPublisher.Verify(m => m.PublishBirthCount(It.IsAny<int>()), Times.Once);
                _mockMetricPublisher.Verify(m => m.PublishDeathCount(It.IsAny<int>()), Times.Once);
                _mockMetricPublisher.Verify(m => m.PublishPopulationCount(It.IsAny<int>()), Times.Once);
            }

            [TestMethod]
            [DataRow(2)]
            [DataRow(3)]
            [DataRow(4)]
            public async Task ShouldPublishCorrectDeathCount(int initialEntityCount)
            {
                // Arrange
                int capturedDeathCount = -1;
                _mockMetricPublisher
                    .Setup(m => m.PublishDeathCount(It.IsAny<int>()))
                    .Callback<int>(count => capturedDeathCount = count)
                    .Returns(Task.CompletedTask);

                Mock<IProximityRule> mockStayAliveRule = new Mock<IProximityRule>();
                mockStayAliveRule.Setup(r => r.HasSufficientNeighbors(It.IsAny<byte>())).Returns(false);
                
                _mockEvolutionRules.Setup(r => r.StayAliveRules).Returns(new[] { mockStayAliveRule.Object });
                _mockEvolutionRules.Setup(r => r.BirthRules).Returns(Array.Empty<IProximityRule>());

                List<Entity> entities = new List<Entity>();
                for (int i = 0; i < initialEntityCount; i++)
                {
                    entities.Add(new Entity(i, 0));
                }
                await _world.InitializeAsync(entities);

                // Act
                await _world.EvolveAsync();

                // Assert
                Assert.AreEqual(initialEntityCount, capturedDeathCount);
            }

            [TestMethod]
            public async Task ShouldPublishZeroDeathCountWhenNoDeaths()
            {
                // Arrange
                int capturedDeathCount = -1;
                _mockMetricPublisher
                    .Setup(m => m.PublishDeathCount(It.IsAny<int>()))
                    .Callback<int>(count => capturedDeathCount = count)
                    .Returns(Task.CompletedTask);

                Mock<IProximityRule> mockStayAliveRule = new Mock<IProximityRule>();
                mockStayAliveRule.Setup(r => r.HasSufficientNeighbors(It.IsAny<byte>())).Returns(true);
                
                _mockEvolutionRules.Setup(r => r.StayAliveRules).Returns(new[] { mockStayAliveRule.Object });
                _mockEvolutionRules.Setup(r => r.BirthRules).Returns(Array.Empty<IProximityRule>());

                await _world.InitializeAsync(Array.Empty<Entity>());

                // Act
                await _world.EvolveAsync();

                // Assert
                Assert.AreEqual(0, capturedDeathCount);
            }

            [TestMethod]
            public async Task ShouldPublishCorrectBirthCount()
            {
                // Arrange
                int capturedBirthCount = -1;
                _mockMetricPublisher
                    .Setup(m => m.PublishBirthCount(It.IsAny<int>()))
                    .Callback<int>(count => capturedBirthCount = count)
                    .Returns(Task.CompletedTask);

                Mock<IProximityRule> mockStayAliveRule = new Mock<IProximityRule>();
                mockStayAliveRule.Setup(r => r.HasSufficientNeighbors(It.IsAny<byte>())).Returns(true);
                
                Mock<IProximityRule> mockBirthRule = new Mock<IProximityRule>();
                mockBirthRule.Setup(r => r.HasSufficientNeighbors(3)).Returns(true);
                
                _mockEvolutionRules.Setup(r => r.StayAliveRules).Returns(new[] { mockStayAliveRule.Object });
                _mockEvolutionRules.Setup(r => r.BirthRules).Returns(new[] { mockBirthRule.Object });

                IEnumerable<Entity> entities = new List<Entity>
                {
                    new Entity(0, 0),
                    new Entity(1, 0),
                    new Entity(0, 1)
                };
                await _world.InitializeAsync(entities);

                // Act
                await _world.EvolveAsync();

                // Assert
                Assert.IsTrue(capturedBirthCount > 0);
            }

            [TestMethod]
            public async Task ShouldPublishCorrectPopulationCount()
            {
                // Arrange
                int capturedPopulationCount = -1;
                _mockMetricPublisher
                    .Setup(m => m.PublishPopulationCount(It.IsAny<int>()))
                    .Callback<int>(count => capturedPopulationCount = count)
                    .Returns(Task.CompletedTask);

                Mock<IProximityRule> mockStayAliveRule = new Mock<IProximityRule>();
                mockStayAliveRule.Setup(r => r.HasSufficientNeighbors(2)).Returns(true);
                mockStayAliveRule.Setup(r => r.HasSufficientNeighbors(3)).Returns(true);
                
                _mockEvolutionRules.Setup(r => r.StayAliveRules).Returns(new[] { mockStayAliveRule.Object });
                _mockEvolutionRules.Setup(r => r.BirthRules).Returns(Array.Empty<IProximityRule>());

                IEnumerable<Entity> entities = new List<Entity>
                {
                    new Entity(1, 1),
                    new Entity(1, 2),
                    new Entity(2, 1)
                };
                await _world.InitializeAsync(entities);

                // Act
                await _world.EvolveAsync();

                // Assert
                Assert.AreEqual(3, capturedPopulationCount);
            }
        }

        [TestClass]
        public class ConwaysGameOfLifeScenarioTests : WorldTests
        {
            [TestInitialize]
            public void SetupMocks()
            {
                SetupMockPublisher();
                SetupConwaysRules();
            }

            private void SetupMockPublisher()
            {
                _mockMetricPublisher
                    .Setup(m => m.PublishBirthCount(It.IsAny<int>()))
                    .Returns(Task.CompletedTask);
                _mockMetricPublisher
                    .Setup(m => m.PublishDeathCount(It.IsAny<int>()))
                    .Returns(Task.CompletedTask);
                _mockMetricPublisher
                    .Setup(m => m.PublishPopulationCount(It.IsAny<int>()))
                    .Returns(Task.CompletedTask);
            }

            public void SetupConwaysRules()
            {
                Mock<IProximityRule> mockStayAliveRule2 = new();
                mockStayAliveRule2.Setup(r => r.HasSufficientNeighbors(2)).Returns(true);
                
                Mock<IProximityRule> mockStayAliveRule3 = new();
                mockStayAliveRule3.Setup(r => r.HasSufficientNeighbors(3)).Returns(true);
                
                Mock<IProximityRule> mockBirthRule = new();
                mockBirthRule.Setup(r => r.HasSufficientNeighbors(3)).Returns(true);
                
                _mockEvolutionRules.Setup(r => r.StayAliveRules)
                    .Returns([mockStayAliveRule2.Object, mockStayAliveRule3.Object]);
                _mockEvolutionRules.Setup(r => r.BirthRules)
                    .Returns([mockBirthRule.Object]);
            }

            [TestMethod]
            public async Task ShouldEvolveBlockPatternToStableState()
            {
                // Arrange - Block pattern (2x2 square)
                IEnumerable<Entity> blockPattern = new List<Entity>
                {
                    new Entity(1, 1),
                    new Entity(1, 2),
                    new Entity(2, 1),
                    new Entity(2, 2)
                };
                await _world.InitializeAsync(blockPattern);

                // Act
                bool evolved = await _world.EvolveAsync();
                IEnumerable<Entity> livingEntities = _world.GetLivingEntities();

                // Assert - Block pattern is stable
                Assert.IsFalse(evolved);
                Assert.AreEqual(4, livingEntities.Count());
            }

            [TestMethod]
            public async Task ShouldEvolveBlinkerPatternCorrectly()
            {
                // Arrange - Horizontal blinker
                IEnumerable<Entity> horizontalBlinker = new List<Entity>
                {
                    new Entity(1, 2),
                    new Entity(2, 2),
                    new Entity(3, 2)
                };
                await _world.InitializeAsync(horizontalBlinker);

                // Act
                bool evolved = await _world.EvolveAsync();
                IEnumerable<Entity> livingEntities = _world.GetLivingEntities();

                // Assert - Should evolve to vertical blinker
                Assert.IsTrue(evolved);
                Assert.AreEqual(3, livingEntities.Count());
                Assert.IsTrue(livingEntities.Contains(new Entity(2, 1)));
                Assert.IsTrue(livingEntities.Contains(new Entity(2, 2)));
                Assert.IsTrue(livingEntities.Contains(new Entity(2, 3)));
            }

            [TestMethod]
            public async Task ShouldKillLonelyCell()
            {
                // Arrange - Single cell with no neighbors
                IEnumerable<Entity> loneCell = new List<Entity> { new Entity(5, 5) };
                await _world.InitializeAsync(loneCell);

                // Act
                await _world.EvolveAsync();
                IEnumerable<Entity> livingEntities = _world.GetLivingEntities();

                // Assert
                Assert.AreEqual(0, livingEntities.Count());
            }

            [TestMethod]
            public async Task ShouldKillOvercrowdedCell()
            {
                // Arrange - Center cell with 4+ neighbors
                IEnumerable<Entity> overcrowded = new List<Entity>
                {
                    new Entity(1, 1), // center
                    new Entity(0, 0),
                    new Entity(0, 1),
                    new Entity(1, 0),
                    new Entity(2, 0)
                };
                await _world.InitializeAsync(overcrowded);

                // Act
                await _world.EvolveAsync();
                IEnumerable<Entity> livingEntities = _world.GetLivingEntities();

                // Assert - Center cell should die due to overcrowding
                Assert.IsFalse(livingEntities.Contains(new Entity(1, 1)));
            }
        }
    }
}
