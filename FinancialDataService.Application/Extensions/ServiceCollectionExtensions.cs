using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialDataService.Application.Extensions
{
    /// <summary>
    /// Extends the <see cref="IServiceCollection"/> with methods for adding application services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }


    }
}