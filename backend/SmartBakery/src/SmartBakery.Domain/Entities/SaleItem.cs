using SmartBakery.Domain.Common;

namespace SmartBakery.Domain.Entities;

public sealed class SaleItem : Entity
{
    public Guid SaleId { get; private set; }

    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;

    public int Quantity { get; private set; }

    // Copia del precio del producto EN EL MOMENTO de la venta. Si mañana el
    // producto sube de precio, esta venta histórica conserva el precio original.
    public decimal UnitPrice { get; private set; }

    public decimal DiscountAmount { get; private set; }
    public decimal Subtotal { get; private set; }

    private SaleItem() { }

    // internal: solo Sale (que está en este mismo proyecto) puede crear items.
    internal static SaleItem Create(Guid saleId, Product product, int quantity, decimal discountAmount)
    {
        if (!product.IsActive)
            throw new DomainException($"Product '{product.Name}' is not active and cannot be sold.");

        Guard.AgainstNegativeOrZero(quantity, nameof(Quantity));
        Guard.AgainstNegative(discountAmount, nameof(DiscountAmount));

        var gross = Money.Round(quantity * product.Price);
        var discount = Money.Round(discountAmount);

        if (discount > gross)
            throw new DomainException($"Discount for '{product.Name}' cannot exceed its amount.");

        // Solo se asigna ProductId (no la navegación Product) para que EF Core
        // no intente insertar de nuevo el producto, que ya existe.
        return new SaleItem
        {
            SaleId = saleId,
            ProductId = product.Id,
            Quantity = quantity,
            UnitPrice = product.Price,
            DiscountAmount = discount,
            Subtotal = gross - discount
        };
    }
}
