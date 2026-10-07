using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SmartBakery.Domain.Common;

namespace SmartBakery.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Se ejecuta justo antes de cada SaveChanges y asigna CreatedAt / UpdatedAt.
/// Así ningún handler tiene que acordarse de hacerlo.
/// </summary>
public sealed class AuditableEntityInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateTimestamps(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateTimestamps(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateTimestamps(DbContext? context)
    {
        if (context is null)
            return;

        // TimeProvider en lugar de DateTime.UtcNow: en los tests podremos
        // "congelar" el reloj y verificar fechas exactas.
        var now = timeProvider.GetUtcNow().UtcDateTime;

        foreach (var entry in context.ChangeTracker.Entries<IHasCreatedAt>())
        {
            // EF puede escribir propiedades con "private set": por eso el
            // dominio no necesita exponer setters públicos para esto.
            if (entry.State == EntityState.Added)
                entry.Property(nameof(IHasCreatedAt.CreatedAt)).CurrentValue = now;

            if (entry.Entity is IHasUpdatedAt && entry.State is EntityState.Added or EntityState.Modified)
                entry.Property(nameof(IHasUpdatedAt.UpdatedAt)).CurrentValue = now;
        }
    }
}
