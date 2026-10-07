using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartBakery.Domain.Entities;

namespace SmartBakery.Infrastructure.Persistence.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    // IDs fijos para los datos semilla: deben ser siempre los mismos o EF
    // pensaría que son filas nuevas en cada migración.
    public static readonly Guid AdminRoleId = Guid.Parse("01920000-0000-7000-8000-000000000001");
    public static readonly Guid EmployeeRoleId = Guid.Parse("01920000-0000-7000-8000-000000000002");

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name).HasMaxLength(Role.NameMaxLength).IsRequired();
        builder.Property(r => r.Description).HasMaxLength(Role.DescriptionMaxLength);

        builder.HasIndex(r => r.Name).IsUnique();

        // Seed data: los roles son parte del sistema, no datos del usuario,
        // así que se crean con la propia migración.
        builder.HasData(
            new { Id = AdminRoleId, Name = Role.Admin, Description = (string?)"Full access: products, sales, models and users." },
            new { Id = EmployeeRoleId, Name = Role.Employee, Description = (string?)"Registers sales and views products and predictions." });
    }
}
