using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBakery.Domain.Entities;

namespace SmartBakery.Infrastructure.Persistence.Configurations;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(Category.NameMaxLength).IsRequired();

        // Sin HasMaxLength: en PostgreSQL se crea como TEXT.
        builder.Property(c => c.Description);

        builder.HasIndex(c => c.Name).IsUnique();
    }
}
