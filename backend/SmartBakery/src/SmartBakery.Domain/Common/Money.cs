namespace SmartBakery.Domain.Common;

/// <summary>
/// Reglas comunes para cantidades de dinero (NUMERIC(10,2) en la base de datos).
/// </summary>
internal static class Money
{
    public const int Decimals = 2;

    public static decimal Round(decimal amount) =>
        Math.Round(amount, Decimals, MidpointRounding.AwayFromZero);

    public static decimal EnsureValidAmount(decimal amount, string field)
    {
        if (Math.Round(amount, Decimals) != amount)
            throw new DomainException($"{field} must have at most {Decimals} decimal places.");

        return amount;
    }
}
