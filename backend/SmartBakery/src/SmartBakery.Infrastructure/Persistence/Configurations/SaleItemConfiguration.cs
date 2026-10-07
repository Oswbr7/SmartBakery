using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBakery.Domain.Entities;

namespace SmartBakery.Infrastructure.Persistence.Configurations;

internal sealed class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("sale_items", t =>
        {
            t.HasCheckConstraint("ck_sale_items_quantity_positive", "quantity > 0");
            t.HasCheckConstraint("ck_sale_items_unit_price_non_negative", "unit_price >= 0");
            t.HasCheckConstraint("ck_sale_items_discount_non_negative", "discount_amount >= 0");
            t.HasCheckConstraint("ck_sale_items_subtotal_non_negative", "subtotal >= 0");
        });

        builder.HasKey(i => i.Id);

        builder.Property(i => i.UnitPrice).HasPrecision(10, 2);
        builder.Property(i => i.DiscountAmount).HasPrecision(10, 2);
        builder.Property(i => i.Subtotal).HasPrecision(12, 2);

        // Restrict: un producto que aparece en ventas no se puede borrar
        // físicamente (por eso existe Product.Deactivate()).
        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
