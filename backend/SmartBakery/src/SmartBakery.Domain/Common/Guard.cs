namespace SmartBakery.Domain.Common;

/// <summary>
/// Validaciones reutilizables para proteger las invariantes de las entidades.
/// Es internal: solo el propio dominio lo usa.
/// </summary>
internal static class Guard
{
    public static string AgainstNullOrWhiteSpace(string? value, string field, int? maxLength = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"{field} is required.");

        var trimmed = value.Trim();

        if (maxLength is int max && trimmed.Length > max)
            throw new DomainException($"{field} must not exceed {max} characters.");

        return trimmed;
    }

    public static string? OptionalText(string? value, string field, int? maxLength = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var trimmed = value.Trim();

        if (maxLength is int max && trimmed.Length > max)
            throw new DomainException($"{field} must not exceed {max} characters.");

        return trimmed;
    }

    public static Guid AgainstEmpty(Guid value, string field)
    {
        if (value == Guid.Empty)
            throw new DomainException($"{field} is required.");

        return value;
    }

    public static int AgainstNegativeOrZero(int value, string field)
    {
        if (value <= 0)
            throw new DomainException($"{field} must be greater than zero.");

        return value;
    }

    public static decimal AgainstNegativeOrZero(decimal value, string field)
    {
        if (value <= 0)
            throw new DomainException($"{field} must be greater than zero.");

        return value;
    }

    public static decimal AgainstNegative(decimal value, string field)
    {
        if (value < 0)
            throw new DomainException($"{field} cannot be negative.");

        return value;
    }

    // PostgreSQL (timestamptz) y Npgsql trabajan en UTC. Exigirlo aquí evita
    // errores sutiles de zona horaria (México es UTC-6) en fechas de ventas.
    public static DateTime AgainstNonUtc(DateTime value, string field)
    {
        if (value.Kind != DateTimeKind.Utc)
            throw new DomainException($"{field} must be expressed in UTC.");

        return value;
    }
}
