using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using service.Interfaces.Infra;
using StackExchange.Redis;

namespace infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddSingleton<IOrderEventStream, InMemoryOrderEventStream>();
            services.AddSingleton<IConnectionMultiplexer>(
                _ => ConnectionMultiplexer.Connect("localhost:6379"));
            services.AddSingleton<IOrderEventStream, RedisOrderEventStream>();
            return services;
        }
    }
}
