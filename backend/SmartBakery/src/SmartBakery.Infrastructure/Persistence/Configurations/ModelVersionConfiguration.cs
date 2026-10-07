using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBakery.Domain.Entities;

namespace SmartBakery.Infrastructure.Persistence.Configurations;

internal sealed class ModelVersionConfiguration : IEntityTypeConfiguration<ModelVersion>
{
    public void Configure(EntityTypeBuilder<ModelVersion> builder)
    {
        builder.ToTable("model_versions");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Version).HasMaxLength(ModelVersion.VersionMaxLength).IsRequired();
        builder.Property(m => m.Algorithm).HasMaxLength(ModelVersion.AlgorithmMaxLength).IsRequired();
        builder.Property(m => m.DatasetVersion).HasMaxLength(ModelVersion.DatasetVersionMaxLength).IsRequired();
        builder.Property(m => m.ModelPath).IsRequired();

        // El enum se guarda como texto ('Candidate', 'Production'...) y no como
        // número: la tabla se entiende sola al consultarla con SQL.
        builder.Property(m => m.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        // IsProduction se calcula a partir de Status: no es una columna.
        builder.Ignore(m => m.IsProduction);

        builder.HasIndex(m => m.Version).IsUnique();
        builder.HasIndex(m => m.TrainingDate);

        // Hay DOS índices sobre la misma columna, así que cada uno necesita un
        // nombre en el modelo de EF (primer argumento de HasIndex) y además un
        // nombre en PostgreSQL (HasDatabaseName). Sin este último, la convención
        // snake_case genera "ix_model_versions_status" e "..._status1".
        builder.HasIndex(m => m.Status, "IX_ModelVersions_Status")
            .HasDatabaseName("ix_model_versions_status");

        // Índice único PARCIAL: solo aplica a las filas en Production.
        // Resultado: PostgreSQL impide que existan dos modelos en producción.
        builder.HasIndex(m => m.Status, "UX_ModelVersions_SingleProduction")
            .IsUnique()
            .HasFilter("status = 'Production'")
            .HasDatabaseName("ux_model_versions_single_production");
    }
}
