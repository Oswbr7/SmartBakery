using SmartBakery.Domain.Common;

namespace SmartBakery.Domain.Entities;

public sealed class Role : Entity
{
    public const string Admin = "Admin";
    public const string Employee = "Employee";

    public const int NameMaxLength = 50;
    public const int DescriptionMaxLength = 255;

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    // Constructor vacío para EF Core. Es privado: el resto del código
    // debe usar Create(), que es donde viven las reglas.
    private Role() { }

    public static Role Create(string name, string? description = null) => new()
    {
        Name = Guard.AgainstNullOrWhiteSpace(name, nameof(Name), NameMaxLength),
        Description = Guard.OptionalText(description, nameof(Description), DescriptionMaxLength)
    };
}
