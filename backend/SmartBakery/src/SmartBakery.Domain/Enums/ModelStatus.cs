namespace SmartBakery.Domain.Enums;

/// <summary>
/// Ciclo de vida de un modelo de Machine Learning.
/// Candidate → Production → Retired, o bien Candidate → Rejected.
/// </summary>
public enum ModelStatus
{
    Candidate = 1,
    Production = 2,
    Rejected = 3,
    Retired = 4
}
