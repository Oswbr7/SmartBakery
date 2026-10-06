using SmartBakery.Domain.Common;

namespace SmartBakery.Domain.Entities;

/// <summary>
/// Una venta completa. Es la "raíz" de sus SaleItems: los items solo se
/// crean a través de la venta, así los totales siempre son consistentes.
/// Una venta es inmutable: no hay métodos para modificarla después de crearla.
/// </summary>
public sealed class Sale : Entity, IHasCreatedAt
{
    private readonly List<SaleItem> _items = [];

    // Cuándo OCURRIÓ la venta (dato de negocio). No es lo mismo que CreatedAt,
    // que indica cuándo se REGISTRÓ en el sistema.
    public DateTime SaleDate { get; private set; }

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public decimal Subtotal { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Desde fuera solo se puede leer la lista, nunca agregar o quitar items.
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

    private Sale() { }

    /// <param name="discountAmount">Descuento aplicado a la venta completa (además de los descuentos por item).</param>
    public static Sale Create(Guid userId, DateTime saleDate, IReadOnlyCollection<SaleLine> lines, decimal discountAmount = 0)
    {
        if (lines is null || lines.Count == 0)
            throw new DomainException("A sale must contain at least one item.");

        var sale = new Sale
        {
            UserId = Guard.AgainstEmpty(userId, nameof(UserId)),
            SaleDate = Guard.AgainstNonUtc(saleDate, nameof(SaleDate))
        };

        foreach (var line in lines)
            sale.AddItem(line);

        sale.CalculateTotals(discountAmount);
        return sale;
    }

    private void AddItem(SaleLine line)
    {
        ArgumentNullException.ThrowIfNull(line.Product);

        if (_items.Any(item => item.ProductId == line.Product.Id))
            throw new DomainException($"Product '{line.Product.Name}' appears more than once in the sale.");

        _items.Add(SaleItem.Create(Id, line.Product, line.Quantity, line.DiscountAmount));
    }

    // Los totales SIEMPRE se calculan aquí. El frontend nunca es fuente
    // confiable para cálculos financieros.
    private void CalculateTotals(decimal discountAmount)
    {
        Guard.AgainstNegative(discountAmount, nameof(DiscountAmount));

        Subtotal = Money.Round(_items.Sum(item => item.Subtotal));
        DiscountAmount = Money.Round(discountAmount);

        if (DiscountAmount > Subtotal)
            throw new DomainException("Discount cannot exceed the sale subtotal.");

        TotalAmount = Subtotal - DiscountAmount;
    }
}

/// <summary>
/// Datos de entrada para cada renglón de una venta.
/// Recibe el Product completo (no solo su Id) para poder copiar su precio actual.
/// </summary>
public sealed record SaleLine(Product Product, int Quantity, decimal DiscountAmount = 0);
