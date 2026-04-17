using System.Text.Json.Serialization;

namespace DentalChart.Core.Entities;

/// <summary>
/// Represents a patient's dental chart (maps to the dental_charts table).
/// Named PatientChart to avoid conflict with the DentalChart namespace.
/// </summary>
public class PatientChart
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string DentitionMode { get; set; } = "permanent";
    public string NotationSystem { get; set; } = "universal";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonIgnore]
    public Patient? Patient { get; set; }
    [JsonIgnore]
    public ICollection<ToothCondition> ToothConditions { get; set; } = [];
}
