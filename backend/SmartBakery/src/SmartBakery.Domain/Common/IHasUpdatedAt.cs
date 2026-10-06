namespace SmartBakery.Domain.Common;

/// <summary>
/// Marca las entidades que además registran su última modificación.
/// </summary>
public interface IHasUpdatedAt : IHasCreatedAt
{
    DateTime UpdatedAt { get; }
}
