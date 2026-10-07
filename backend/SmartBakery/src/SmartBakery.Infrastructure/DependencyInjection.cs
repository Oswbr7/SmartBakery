using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartBakery.Infrastructure.Persistence;
using SmartBakery.Infrastructure.Persistence.Interceptors;

namespace SmartBakery.Infrastructure;

/// <summary>
/// Registra todos los servicios de Infrastructure. La API solo llama a
/// AddInfrastructure() y no necesita conocer EF Core ni Npgsql.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<AuditableEntityInterceptor>();

        services.AddDbContext<AppDbContext>((serviceProvider, options) => options
            .UseNpgsql(connectionString)
            // PostgreSQL usa snake_case (sale_items, unit_price). Esta convención
            // traduce automáticamente SaleItem.UnitPrice → sale_items.unit_price.
            .UseSnakeCaseNamingConvention()
            .AddInterceptors(serviceProvider.GetRequiredService<AuditableEntityInterceptor>()));

        return services;
    }
}
