using System.Text.Json.Serialization;

namespace DentalChart.Core.Entities;

public class InsurancePlan
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public int Priority { get; set; } = 1;
    public string CarrierName { get; set; } = string.Empty;
    public string? PlanName { get; set; }
    public string? GroupNumber { get; set; }
    public string MemberId { get; set; } = string.Empty;
    public string? EmployerName { get; set; }
    public string? SubscriberName { get; set; }
    public DateOnly? SubscriberDob { get; set; }
    public string? SubscriberRelationship { get; set; }
    public DateOnly? EffectiveDate { get; set; }
    public DateOnly? TerminationDate { get; set; }
    public decimal? AnnualMaximum { get; set; }
    public decimal? Deductible { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    [JsonIgnore]
    public Patient? Patient { get; set; }
}
