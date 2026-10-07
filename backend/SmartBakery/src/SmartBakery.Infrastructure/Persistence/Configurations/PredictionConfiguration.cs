using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBakery.Domain.Entities;

namespace SmartBakery.Infrastructure.Persistence.Configurations;

internal sealed class PredictionConfiguration : IEntityTypeConfiguration<Prediction>
{
    public void Configure(EntityTypeBuilder<Prediction> builder)
    {
        builder.ToTable("predictions", t =>
            t.HasCheckConstraint("ck_predictions_quantity_non_negative", "predicted_quantity >= 0"));

        builder.HasKey(p => p.Id);

        builder.Property(p => p.PredictedQuantity).HasPrecision(10, 2);

        builder.HasIndex(p => p.PredictionDate);

        builder.HasOne(p => p.Product)
            .WithMany()
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ModelVersion)
            .WithMany()
            .HasForeignKey(p => p.ModelVersionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
