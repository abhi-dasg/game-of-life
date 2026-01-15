using GameOfLife.API.Exceptions;
using GameOfLife.API.Models;
using GameOfLife.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameOfLife.API.Controllers
{
    /// <summary>
    /// Endpoints for the Game of Life API.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class GameOfLifeController : ControllerBase
    {
        private IGameOfLifeService _gameOfLifeService;

        public GameOfLifeController(IGameOfLifeService gameOfLifeService)
        {
            _gameOfLifeService = gameOfLifeService;
        }

        [HttpPost]
        [Produces("text/plain")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateWorld([FromBody] IEnumerable<Coordinate> entityLocations)
        {
            Guid worldIdentifier = await _gameOfLifeService.CreateWorldAsync(entityLocations);

            return StatusCode(StatusCodes.Status201Created, worldIdentifier.ToString());
        }

        [HttpPut("{worldIdentifier}/evolve")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<Coordinate>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Evolve(Guid worldIdentifier, [FromQuery] int generationCount = 1)
        {
            IEnumerable<Coordinate> evolvedWorldEntities = await _gameOfLifeService.EvolveWorldAsync(worldIdentifier, generationCount);

            return Ok(evolvedWorldEntities);
        }

        [HttpPut("{worldIdentifier}/evolve/final")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<Coordinate>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EvolveToFinalState(Guid worldIdentifier)
        {
            try
            {
                IEnumerable<Coordinate> finalWorldEntities = await _gameOfLifeService.EvolveToFinalWorldStateAsync(worldIdentifier);
                return Ok(finalWorldEntities);
            }
            catch (UnableToStabilizePopulationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    error = ex.GetErrorCode(),
                    message = ex.Message,
                    worldIdentifier = worldIdentifier.ToString(),
                    maxGenerations = ex.MaxGenerations
                });
            }
        }
    }
}
