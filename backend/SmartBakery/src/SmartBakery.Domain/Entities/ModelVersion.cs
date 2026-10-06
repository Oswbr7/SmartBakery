using SmartBakery.Domain.Common;
using SmartBakery.Domain.Enums;

namespace SmartBakery.Domain.Entities;

/// <summary>
/// Metadata de un modelo entrenado. El archivo .joblib vive en el ML Service;
/// aquí solo guardamos qué es, de dónde salió y en qué estado está.
/// </summary>
public sealed class ModelVersion : Entity, IHasCreatedAt
{
    public const int VersionMaxLength = 50;
    public const int AlgorithmMaxLength = 100;
    public const int DatasetVersionMaxLength = 100;

    public string Version { get; private set; } = string.Empty;
    public string Algorithm { get; private set; } = string.Empty;

    // Por ahora es texto (p. ej. "sales_2026_10"). Cuando exista la entidad
    // DatasetVersion (Fase de Model Versioning) se convertirá en una FK.
    public string DatasetVersion { get; private set; } = string.Empty;

    public string ModelPath { get; private set; } = string.Empty;
    public ModelStatus Status { get; private set; }
    public DateTime TrainingDate { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Se DERIVA de Status en lugar de guardarse como columna aparte:
    // dos campos que dicen lo mismo pueden terminar contradiciéndose.
    public bool IsProduction => Status == ModelStatus.Production;

    private ModelVersion() { }

    // Todo modelo nace como Candidate. Nunca entra directo a producción.
    public static ModelVersion Create(string version, string algorithm, string datasetVersion, string modelPath, DateTime trainingDate) => new()
    {
        Version = Guard.AgainstNullOrWhiteSpace(version, nameof(Version), VersionMaxLength),
        Algorithm = Guard.AgainstNullOrWhiteSpace(algorithm, nameof(Algorithm), AlgorithmMaxLength),
        DatasetVersion = Guard.AgainstNullOrWhiteSpace(datasetVersion, nameof(DatasetVersion), DatasetVersionMaxLength),
        ModelPath = Guard.AgainstNullOrWhiteSpace(modelPath, nameof(ModelPath)),
        TrainingDate = Guard.AgainstNonUtc(trainingDate, nameof(TrainingDate)),
        Status = ModelStatus.Candidate
    };

    // Las transiciones válidas forman una pequeña máquina de estados:
    //   Candidate → Production → Retired
    //   Candidate → Rejected
    public void Promote() => TransitionTo(ModelStatus.Production, from: ModelStatus.Candidate);

    public void Reject() => TransitionTo(ModelStatus.Rejected, from: ModelStatus.Candidate);

    public void Retire() => TransitionTo(ModelStatus.Retired, from: ModelStatus.Production);

    private void TransitionTo(ModelStatus target, ModelStatus from)
    {
        if (Status != from)
            throw new DomainException($"Model {Version} cannot change from {Status} to {target}.");

        Status = target;
    }
}
