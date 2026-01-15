using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Common.Web.Test
{
    /// <summary>
    /// Provides a base class for creating test web application factories with customizable environment and
    /// configuration settings for integration testing.
    /// </summary>
    /// <remarks>Use this class to configure the test environment and application settings when running
    /// integration tests. The <see cref="Environment"/> property determines the environment name used during test
    /// execution, allowing tests to simulate different deployment scenarios. Override the <c>Configure</c> method to
    /// customize configuration sources, such as adding test-specific configuration files or environment
    /// variables.</remarks>
    /// <typeparam name="T">The entry point class of the ASP.NET Core application to be tested.</typeparam>
    public abstract class TestWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
    {
        public string Environment { get; set; } = "Development";

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment(Environment)
                   .ConfigureHostConfiguration(Configure);

            return base.CreateHost(builder);
        }

        protected virtual void Configure(IConfigurationBuilder config)
        {
            config.AddJsonFile("appsettings.json", optional: true)
                  .AddEnvironmentVariables();
        }

        protected override void ConfigureClient(HttpClient client)
        {
            base.ConfigureClient(client);
            client.Timeout = TimeSpan.FromMinutes(5.0);
        }
    }
}
