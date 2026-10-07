using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBakery.Domain.Entities;

namespace SmartBakery.Infrastructure.Persistence.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Check constraint: la misma regla que Product.ChangePrice, repetida
        // en la base de datos como última red de seguridad.
        builder.ToTable("products", t =>
            t.HasCheckConstraint("ck_products_price_positive", "price > 0"));

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).HasMaxLength(Product.NameMaxLength).IsRequired();
        builder.Property(p => p.Description);

        // NUMERIC(10,2): dinero exacto. Nunca usar float/double para dinero.
        builder.Property(p => p.Price).HasPrecision(10, 2);

        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.IsActive);

        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
