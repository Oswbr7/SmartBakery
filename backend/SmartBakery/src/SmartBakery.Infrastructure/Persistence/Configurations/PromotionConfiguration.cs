using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBakery.Domain.Entities;

namespace SmartBakery.Infrastructure.Persistence.Configurations;

internal sealed class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder.ToTable("promotions", t =>
        {
            t.HasCheckConstraint(
                "ck_promotions_discount_range",
                "discount_percentage > 0 AND discount_percentage <= 100");
            t.HasCheckConstraint("ck_promotions_dates", "end_date >= start_date");
        });

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).HasMaxLength(Promotion.NameMaxLength).IsRequired();
        builder.Property(p => p.Description);
        builder.Property(p => p.DiscountPercentage).HasPrecision(5, 2);

        // DateOnly se mapea automáticamente a DATE en PostgreSQL.
        builder.Property(p => p.StartDate);
        builder.Property(p => p.EndDate);
    }
}
