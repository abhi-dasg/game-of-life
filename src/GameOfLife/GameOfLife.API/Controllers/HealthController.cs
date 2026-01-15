using Microsoft.AspNetCore.Mvc;

namespace GameOfLife.API.Controllers
{
    /// <summary>
    /// Endpoints for health checks.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private const string HealthyResponse = "Healthy";

        /// <summary>
        /// Returns a plain text response indicating the health status of the service.
        /// </summary>
        /// <returns>A string containing the health status message. Typically returns "Healthy" to indicate the service is
        /// operational.</returns>
        [HttpGet]
        [Produces("text/plain")]
        public async Task<string> GetSimpleHealth()
        {
            return HealthyResponse;
        }
    }
}
