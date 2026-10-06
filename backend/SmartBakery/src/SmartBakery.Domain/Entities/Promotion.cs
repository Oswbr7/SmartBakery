using SmartBakery.Domain.Common;

namespace SmartBakery.Domain.Entities;

public sealed class Promotion : Entity, IHasCreatedAt
{
    public const int NameMaxLength = 150;

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal DiscountPercentage { get; private set; }

    // DateOnly (DATE en PostgreSQL): una promoción vale por días completos,
    // no necesita hora ni zona horaria.
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }

    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Promotion() { }

    public static Promotion Create(
        string name,
        string? description,
        decimal discountPercentage,
        DateOnly startDate,
        DateOnly endDate)
    {
        if (discountPercentage <= 0 || discountPercentage > 100)
            throw new DomainException("Discount percentage must be greater than 0 and at most 100.");

        if (endDate < startDate)
            throw new DomainException("End date cannot be earlier than start date.");

        return new Promotion
        {
            Name = Guard.AgainstNullOrWhiteSpace(name, nameof(Name), NameMaxLength),
            Description = Guard.OptionalText(description, nameof(Description)),
            DiscountPercentage = Money.EnsureValidAmount(discountPercentage, nameof(DiscountPercentage)),
            StartDate = startDate,
            EndDate = endDate,
            IsActive = true
        };
    }

    public bool IsApplicableOn(DateOnly date) =>
        IsActive && date >= StartDate && date <= EndDate;

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
