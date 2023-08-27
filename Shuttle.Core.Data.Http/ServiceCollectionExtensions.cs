using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Core.Data.Http
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpDatabaseContextService(this IServiceCollection services)
        {
            Guard.AgainstNull(services, nameof(services));

            services.AddSingleton<IDatabaseContextService, ContextDatabaseContextService>();

            return services;
        }
    }
}