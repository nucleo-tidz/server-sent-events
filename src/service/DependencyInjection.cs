using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using service.Interfaces.Services;

namespace service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IOrderService, OrderService>();
            return services;
        }
    }
}
