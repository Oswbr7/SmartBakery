using Microsoft.EntityFrameworkCore;
using SmartBakery.Domain.Entities;

namespace SmartBakery.Infrastructure.Persistence;

/// <summary>
/// Punto de entrada de EF Core a la base de datos.
/// Cada DbSet representa una tabla; el mapeo detallado vive en Configurations/.
/// </summary>
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<Prediction> Predictions => Set<Prediction>();
    public DbSet<ModelVersion> ModelVersions => Set<ModelVersion>();

    // Busca automáticamente todas las clases IEntityTypeConfiguration<T>
    // de este proyecto. Así el DbContext no crece con cada entidad nueva.
    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
