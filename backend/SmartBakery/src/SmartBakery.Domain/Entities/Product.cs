using SmartBakery.Domain.Common;

namespace SmartBakery.Domain.Entities;

public sealed class Product : Entity, IHasUpdatedAt
{
    public const int NameMaxLength = 150;

    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Product() { }

    public static Product Create(Guid categoryId, string name, string? description, decimal price)
    {
        var product = new Product { IsActive = true };
        product.Update(categoryId, name, description);
        product.ChangePrice(price);
        return product;
    }

    public void Update(Guid categoryId, string name, string? description)
    {
        CategoryId = Guard.AgainstEmpty(categoryId, nameof(CategoryId));
        Name = Guard.AgainstNullOrWhiteSpace(name, nameof(Name), NameMaxLength);
        Description = Guard.OptionalText(description, nameof(Description));
    }

    // El precio se cambia con un método propio y no dentro de Update():
    // en una fase posterior este será el punto donde se registre ProductPriceHistory.
    public void ChangePrice(decimal price)
    {
        Guard.AgainstNegativeOrZero(price, nameof(Price));
        Price = Money.EnsureValidAmount(price, nameof(Price));
    }

    public void Activate() => IsActive = true;

    // Un producto con ventas históricas no se borra: se desactiva.
    public void Deactivate() => IsActive = false;
}
