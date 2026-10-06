namespace SmartBakery.Domain.Common;

/// <summary>
/// Se lanza cuando una operación violaría una regla de negocio.
/// El middleware global de la API la convertirá en una respuesta 400.
/// </summary>
public class DomainException(string message) : Exception(message);
