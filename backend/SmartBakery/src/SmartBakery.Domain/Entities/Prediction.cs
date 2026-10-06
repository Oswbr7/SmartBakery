using SmartBakery.Domain.Common;

namespace SmartBakery.Domain.Entities;

public sealed class Prediction : Entity, IHasCreatedAt
{
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;

    // Día para el que se predice la demanda.
    public DateOnly PredictionDate { get; private set; }

    // Decimal y no int: el modelo produce valores continuos (p. ej. 17.4).
    // Redondear a unidades es una decisión de presentación o de negocio.
    public decimal PredictedQuantity { get; private set; }

    // Trazabilidad: toda predicción sabe qué modelo la generó.
    public Guid ModelVersionId { get; private set; }
    public ModelVersion ModelVersion { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

    private Prediction() { }

    public static Prediction Create(Guid productId, DateOnly predictionDate, decimal predictedQuantity, Guid modelVersionId) => new()
    {
        ProductId = Guard.AgainstEmpty(productId, nameof(ProductId)),
        PredictionDate = predictionDate,
        PredictedQuantity = Guard.AgainstNegative(predictedQuantity, nameof(PredictedQuantity)),
        ModelVersionId = Guard.AgainstEmpty(modelVersionId, nameof(ModelVersionId))
    };
}
