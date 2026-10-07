using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBakery.Domain.Entities;

namespace SmartBakery.Infrastructure.Persistence.Configurations;

internal sealed class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("sales", t =>
        {
            t.HasCheckConstraint("ck_sales_subtotal_non_negative", "subtotal >= 0");
            t.HasCheckConstraint("ck_sales_discount_non_negative", "discount_amount >= 0");
            t.HasCheckConstraint("ck_sales_total_non_negative", "total_amount >= 0");
            t.HasCheckConstraint("ck_sales_total_consistent", "total_amount = subtotal - discount_amount");
        });

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Subtotal).HasPrecision(12, 2);
        builder.Property(s => s.DiscountAmount).HasPrecision(12, 2);
        builder.Property(s => s.TotalAmount).HasPrecision(12, 2);

        // Consultas por rango de fechas (dashboard, dataset de ML) serán muy frecuentes.
        builder.HasIndex(s => s.SaleDate);

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Sale es dueña de sus items: si (excepcionalmente) se borrara una
        // venta, sus items se van con ella.
        builder.HasMany(s => s.Items)
            .WithOne()
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        // "Items" es de solo lectura; EF debe leer y escribir el campo privado _items.
        builder.Navigation(s => s.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
