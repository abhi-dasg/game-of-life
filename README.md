# Game of Life

A configurable implementation of Conway's Game of Life and its variants built with .NET 10.

## Overview

This API provides a flexible Game of Life simulation with fully configurable evolution rules. You can implement classic Conway's Game of Life or create custom cellular automaton variations.

## Architecture

The application is designed with flexibility and scalability in mind, offering multiple implementation strategies for data persistence and metrics publishing.

### World Repository Implementations

The application provides three repository implementations for storing and managing world states. Configure your preferred implementation in `ServiceExtensions.cs`:

#### 1. InMemoryWorldRepository (Default - Development Only)
```csharp
services.AddSingleton<IWorldRepository, InMemoryWorldRepository>();
```
- **Use Case**: Development, testing, and demos
- **Characteristics**:
  - Stores worlds in application memory
  - Fast performance
  - No persistence (data lost on restart)
  - Not thread-safe
  - **NOT suitable for production**
  - Does not support horizontal scaling
  - Does not support fault tolerance

#### 2. SharedCacheWorldRepository (Recommended for Production)
```csharp
services.AddSingleton<IWorldRepository, SharedCacheWorldRepository>();
```
- **Use Case**: Production environments requiring scalability
- **Characteristics**:
  - Uses shared cache (e.g., AWS ElastiCache, Azure Redis Cache)
  - Enables horizontal scaling across multiple instances
  - Shared state across service instances
  - Fast read/write performance
  - Requires serialization mechanism (Protocol Buffers recommended)
  - **Status**: Implementation pending

#### 3. DbWorldRepository (Alternative for Production)
```csharp
services.AddSingleton<IWorldRepository, DbWorldRepository>();
```
- **Use Case**: Production environments with persistence requirements
- **Characteristics**:
  - Database-backed storage
  - Full data persistence
  - Supports scaling but slower than shared cache
  - Requires serialization mechanism (Protocol Buffers recommended)
  - **Status**: Implementation pending

### Metric Publisher Implementations

Choose how simulation metrics (births, deaths, population) are published:

#### 1. LoggingMetricPublisher (Default - Development)
```csharp
services.AddSingleton<IMetricPublisher, LoggingMetricPublisher>();
```
- **Use Case**: Development and debugging
- **Characteristics**:
  - Writes metrics to application logs
  - Logs at Information level
  - Simple implementation
  - Suitable for development and troubleshooting

#### 2. MonitoringMetricPublisher (Recommended for Production)
```csharp
services.AddSingleton<IMetricPublisher, MonitoringMetricPublisher>();
```
- **Use Case**: Production monitoring and analytics
- **Characteristics**:
  - Publishes to monitoring systems (e.g., CloudWatch, Application Insights, Prometheus)
  - Enables real-time operational metrics
  - Supports dashboards and alerting
  - **Status**: Implementation pending

### Evolution Rules Implementations

The application supports two evolution rule strategies:

#### 1. ConwaysEvolutionRules (Default)
```csharp
UseConwaysRules(services);
```
- Implements classic Conway's Game of Life rules
- Hardcoded: Stay alive with 2-3 neighbors, birth with exactly 3 neighbors

#### 2. ConfigurableEvolutionRules
```csharp
UseConfigurableRules(services);
```
- Fully configurable rules via `appsettings.json`
- Enables custom cellular automaton variations
- See configuration section below for details

## Configuration

The Game of Life evolution rules and settings are fully configurable through `appsettings.json` under the `GameOfLife` section.

### Configuration Structure

```json
{
  "GameOfLife": {
    "MaxAutoEvolution": 1000,
    "EvolutionRules": {
      "StayAliveRules": [
        {
          "Type": "Range",
          "Value": 2,
          "MaxValue": 3
        }
      ],
      "BirthRules": [
        {
          "Type": "Simple",
          "Value": 3
        }
      ]
    }
  }
}
```

### Configuration Options

- **MaxAutoEvolution**: Maximum number of generations that can be evolved automatically
- **EvolutionRules**: Defines the rules for entity birth and survival

## Rule Types

### Simple Rule
Matches an exact neighbor count.

**Example:** Entity is born with exactly 3 neighbors
```json
{
  "Type": "Simple",
  "Value": 3
}
```

### Range Rule
Matches a range of neighbor counts (inclusive).

**Example:** Entity stays alive with 2-3 neighbors
```json
{
  "Type": "Range",
  "Value": 2,
  "MaxValue": 3
}
```

## Multiple Rules

You can define multiple rules for both staying alive and birth conditions:

```json
{
  "GameOfLife": {
    "EvolutionRules": {
      "StayAliveRules": [
        {
          "Type": "Simple",
          "Value": 2
        },
        {
          "Type": "Simple",
          "Value": 3
        },
        {
          "Type": "Simple",
          "Value": 5
        }
      ],
      "BirthRules": [
        {
          "Type": "Range",
          "Value": 3,
          "MaxValue": 4
        }
      ]
    }
  }
}
```

## Environment-Specific Rules

Override rules per environment using `appsettings.{Environment}.json`:

- `appsettings.json` - Default/Production rules
- `appsettings.Development.json` - Development rules
- `appsettings.Personal.json` - Personal local environment rules

## Classic Conway's Game of Life Rules

The default configuration implements the classic Conway's Game of Life:
- **Stay Alive:** 2-3 neighbors
- **Birth:** Exactly 3 neighbors

## Custom Variations

### HighLife
```json
{
  "GameOfLife": {
    "EvolutionRules": {
      "StayAliveRules": [
        { "Type": "Range", "Value": 2, "MaxValue": 3 }
      ],
      "BirthRules": [
        { "Type": "Simple", "Value": 3 },
        { "Type": "Simple", "Value": 6 }
      ]
    }
  }
}
```

### Seeds
```json
{
  "GameOfLife": {
    "EvolutionRules": {
      "StayAliveRules": [],
      "BirthRules": [
        { "Type": "Simple", "Value": 2 }
      ]
    }
  }
}
```

### Day & Night
```json
{
  "GameOfLife": {
    "EvolutionRules": {
      "StayAliveRules": [
        { "Type": "Range", "Value": 3, "MaxValue": 4 },
        { "Type": "Range", "Value": 6, "MaxValue": 8 }
      ],
      "BirthRules": [
        { "Type": "Simple", "Value": 3 },
        { "Type": "Range", "Value": 6, "MaxValue": 8 }
      ]
    }
  }
}
```

## Technologies

- .NET 10
- ASP.NET Core Web API
- AutoMapper
- OpenAPI/Swagger (Development)

## Getting Started

1. Clone the repository
2. Navigate to the `src` directory
3. Configure your preferred implementations in `GameOfLife.API/Extensions/ServiceExtensions.cs`:
   - Choose a repository implementation
   - Choose a metric publisher
   - Choose evolution rules strategy
4. Run the API:
   ```bash
   dotnet run --project GameOfLife/GameOfLife.API
   ```
5. Access the API documentation at `https://localhost:{port}/openapi` (Development mode) (TODO)

## Testing

The solution includes:
- **Unit Tests**: `GameOfLife.API.UnitTests`
- **Functional Tests**: `GameOfLife.API.FunctionalTests`

Run tests using:
```bash
dotnet test
```

## Production Deployment Considerations

To build it for production:

1. **Switch to SharedCacheWorldRepository** or **DbWorldRepository** for scalability and persistence
2. **Switch to MonitoringMetricPublisher** for operational visibility
3. **Configure MaxAutoEvolution** appropriately for your use case
4. **Implement serialization** (Protocol Buffers recommended) for repository implementations
5. **Set up monitoring and alerting** using the metrics published by MonitoringMetricPublisher
6. **Configure environment-specific settings** via `appsettings.Production.json`
7. **For Single writer and multi reader scaling** Create coordinator service, leader election and follower promotion logic


## Remaining TODO

1. Unit tests for remaining classes with any business logic.
2. Containerize
3. Full CI/CD with branching strategy
4. Open API/Swager spec
