using System.Text.Json.Serialization;

namespace DentalChart.Core.Entities;

public class MedicalHistory
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public bool HasDiabetes { get; set; }
    public bool HasHypertension { get; set; }
    public bool HasHeartDisease { get; set; }
    public bool HasArtificialHeartValve { get; set; }
    public bool HasPacemaker { get; set; }
    public bool HasBloodThinners { get; set; }
    public bool HasBisphosphonates { get; set; }
    public bool HasBleedingDisorder { get; set; }
    public bool HasHiv { get; set; }
    public bool HasHepatitis { get; set; }
    public bool HasEpilepsy { get; set; }
    public bool HasAsthma { get; set; }
    public bool IsPregnant { get; set; }
    public bool IsNursing { get; set; }
    public string? OtherConditions { get; set; }
    public string? CurrentMedications { get; set; }
    public string Allergies { get; set; } = "[]";
    public string AlertFlags { get; set; } = "[]";
    public DateTimeOffset LastUpdated { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonIgnore]
    public Patient? Patient { get; set; }
}
