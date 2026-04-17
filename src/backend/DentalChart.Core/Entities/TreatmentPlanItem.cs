using System.Text.Json.Serialization;

namespace DentalChart.Core.Entities;

public class TreatmentPlanItem
{
    public Guid Id { get; set; }
    public Guid PlanId { get; set; }
    public string? ToothNumber { get; set; }
    public string[]? Surfaces { get; set; }
    public Guid ProcedureCodeId { get; set; }
    public string Status { get; set; } = "planned";
    public Guid? ProviderId { get; set; }
    public decimal? Fee { get; set; }
    public decimal? InsuranceEstimate { get; set; }
    public decimal? PatientPortion { get; set; }
    public int VisitNumber { get; set; } = 1;
    public int SortOrder { get; set; }
    public string? Note { get; set; }
    public Guid? AppointmentId { get; set; }
    public DateOnly? DateCompleted { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    [JsonIgnore]
    public TreatmentPlan? Plan { get; set; }
    public ProcedureCode? ProcedureCode { get; set; }
    public Provider? Provider { get; set; }
}
