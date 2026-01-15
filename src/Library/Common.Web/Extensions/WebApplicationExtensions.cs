using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Common.Web.Extensions
{
    public static class WebApplicationExtensions
    {
        /// <summary>
        /// Configures common middleware and endpoints for the specified web application, including HTTPS redirection
        /// and controller mapping. In development environments, also enables OpenAPI documentation.
        /// </summary>
        /// <remarks>Call this method during application startup to set up standard API features. In
        /// development mode, OpenAPI endpoints are mapped to facilitate API exploration and testing.</remarks>
        /// <param name="app">The web application to configure. Must not be null.</param>
        public static void UseCommonApi(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.MapControllers();

#if DEBUG
            PrintEndpoints(app);
#endif
        }

        private static void PrintEndpoints(WebApplication app)
        {
            ILogger logger = app.Logger;
            IActionDescriptorCollectionProvider actionDescriptorProvider = app.Services.GetRequiredService<IActionDescriptorCollectionProvider>();
            IEnumerable<dynamic> routes = actionDescriptorProvider.ActionDescriptors
                                                                  .Items
                                                                  .Where(x => x.AttributeRouteInfo?.Template != null)
                                                                  .Select(x => new
                                                                      {
                                                                            Template = x.AttributeRouteInfo!.Template,
                                                                            HttpMethods = x.EndpointMetadata.OfType<HttpMethodMetadata>()
                                                                                .SelectMany(m => m.HttpMethods).Distinct()
                                                                      })
                                                                  .OrderBy(x => x.Template);

            logger.LogInformation("Available API endpoints:");
            foreach (dynamic route in routes)
            {
                IEnumerable<string> httpMethods = route.HttpMethods;
                string methods = httpMethods.Any() ? string.Join(", ", httpMethods) : "ALL";
                string path = $"/{route.Template?.TrimStart('/')}";
                logger.LogInformation("{Methods} - {Path}", methods, path);
            }
        }
    }
}
