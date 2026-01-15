using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Common.Web.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        private const string LoggerFormatterName = "simple";

        /// <summary>
        /// Configures common API services for the specified web application builder, including controllers, OpenAPI
        /// documentation, and IIS server options.
        /// </summary>
        /// <remarks>This method adds controller support and OpenAPI documentation to the application's
        /// service collection. It also configures IIS server options to disable automatic authentication. Call this
        /// method during application startup to ensure required API services are registered before building the
        /// application.</remarks>
        /// <param name="builder">The <see cref="WebApplicationBuilder"/> instance to configure. Cannot be null.</param>
        public static void AddCommonApi(this WebApplicationBuilder builder)
        {
            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });
        }

        /// <summary>
        /// Enables console logging for the specified web application builder with a simple, single-line format and
        /// timestamp.
        /// </summary>
        /// <remarks>This method configures the logging system to output log messages to the console using
        /// a simple formatter. Log entries will appear in a single line and include a timestamp in the format 'HH:mm:ss
        /// '.</remarks>
        /// <param name="builder">The WebApplicationBuilder to configure for console logging. Cannot be null.</param>
        public static void AddConsoleLogging(this WebApplicationBuilder builder)
        {
            builder.Logging.AddConsole(options =>
            {
                options.FormatterName = LoggerFormatterName;
            });

            builder.Logging.AddSimpleConsole(options =>
            {
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
            });
        }
    }
}
