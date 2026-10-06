using SmartBakery.Domain.Common;

namespace SmartBakery.Domain.Entities;

public sealed class Category : Entity, IHasUpdatedAt
{
    public const int NameMaxLength = 100;

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Category() { }

    public static Category Create(string name, string? description = null)
    {
        var category = new Category { IsActive = true };
        category.Update(name, description);
        return category;
    }

    public void Update(string name, string? description)
    {
        Name = Guard.AgainstNullOrWhiteSpace(name, nameof(Name), NameMaxLength);
        Description = Guard.OptionalText(description, nameof(Description));
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
