namespace SmartBakery.Domain.Common;

/// <summary>
/// Clase base de todas las entidades: lo único que comparten es su identidad.
/// </summary>
public abstract class Entity
{
    // El Id se genera en el momento de crear el objeto, no en la base de datos.
    // Guid v7 está ordenado por tiempo, así que los índices de PostgreSQL
    // no se fragmentan como con un Guid aleatorio (v4).
    public Guid Id { get; protected init; } = Guid.CreateVersion7();
}
