using System.Text.Json.Serialization;

namespace DentalChart.Core.Entities;

public class ToothCondition
{
    public Guid Id { get; set; }
    public Guid ChartId { get; set; }
    public string ToothNumber { get; set; } = string.Empty;
    public string[]? Surfaces { get; set; }
    public string ConditionType { get; set; } = string.Empty;
    public string Status { get; set; } = "existing_other";
    public Guid? ProcedureCodeId { get; set; }
    public Guid? ProviderId { get; set; }
    public decimal? Fee { get; set; }
    public string? Note { get; set; }
    public DateOnly? DateCompleted { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    [JsonIgnore]
    public PatientChart? Chart { get; set; }
    public ProcedureCode? ProcedureCode { get; set; }
    public Provider? Provider { get; set; }
}
