using GameOfLife.API.Configuration;
using GameOfLife.API.Models;
using GameOfLife.API.Repositories;
using GameOfLife.API.Services;
using GameOfLife.API.Services.Metrics;
using GameOfLife.API.Services.Rules;
using Microsoft.Extensions.Options;

namespace GameOfLife.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<GameOfLifeConfiguration>(configuration.GetSection("GameOfLife"));
            
            services.AddScoped<IGameOfLifeService, GameOfLifeService>();

            services.AddTransient<IWorldFactory, WorldFactory>();
            services.AddTransient<IWorld, World>();


            services.AddSingleton<IWorldRepository, InMemoryWorldRepository>();
            //services.AddSingleton<IWorldRepository, SharedCacheWorldRepository>();
            //services.AddSingleton<IWorldRepository, DbWorldRepository>();

            services.AddSingleton<IMetricPublisher, LoggingMetricPublisher>();
            //services.AddSingleton<IMetricPublisher, MonitoringMetricPublisher>();
            
            UseConwaysRules(services);
            //UseConfigurableRules(services);
        }

        private static void UseConwaysRules(IServiceCollection services)
        {
            services.AddSingleton<IEvolutionRules, ConwaysEvolutionRules>();
        }

        private static void UseConfigurableRules(IServiceCollection services)
        {
            services.AddOptions<EvolutionRulesOptions>()
                    .Configure<IOptions<GameOfLifeConfiguration>>((options, gameOfLifeConfig) =>
                    {
                        if (gameOfLifeConfig.Value.EvolutionRules != null)
                        {
                            options.StayAliveRules = ProximityRuleFactory.CreateRules(gameOfLifeConfig.Value.EvolutionRules.StayAliveRules);
                            options.BirthRules = ProximityRuleFactory.CreateRules(gameOfLifeConfig.Value.EvolutionRules.BirthRules);
                        }
                    });

            services.AddSingleton<IEvolutionRules, ConfigurableEvolutionRules>();
        }
    }
}
