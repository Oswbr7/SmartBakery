namespace SmartBakery.Domain.Common;

/// <summary>
/// Marca las entidades que registran cuándo se guardaron en el sistema.
/// El valor lo asigna Infrastructure al guardar, no el dominio.
/// </summary>
public interface IHasCreatedAt
{
    DateTime CreatedAt { get; }
}
