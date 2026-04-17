using System.Text.Json.Serialization;

namespace DentalChart.Core.Entities;

public class TreatmentPlan
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string Name { get; set; } = "Treatment Plan";
    public string Status { get; set; } = "active";
    public DateOnly? AcceptedDate { get; set; }
    public string? AcceptedSignature { get; set; }
    public string? Notes { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    [JsonIgnore]
    public Patient? Patient { get; set; }
    [JsonIgnore]
    public ICollection<TreatmentPlanItem> Items { get; set; } = [];
}
