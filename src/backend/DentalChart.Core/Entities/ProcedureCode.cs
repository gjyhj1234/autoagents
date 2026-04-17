using System.Text.Json.Serialization;

namespace DentalChart.Core.Entities;

public class ProcedureCode
{
    public Guid Id { get; set; }
    public string AdaCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? AbbreviatedDesc { get; set; }
    public string? Category { get; set; }
    public decimal? DefaultFee { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonIgnore]
    public ICollection<ToothCondition> ToothConditions { get; set; } = [];
    [JsonIgnore]
    public ICollection<TreatmentPlanItem> TreatmentPlanItems { get; set; } = [];
}
