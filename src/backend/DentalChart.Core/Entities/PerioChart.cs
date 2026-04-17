using System.Text.Json.Serialization;

namespace DentalChart.Core.Entities;

public class PerioChart
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public DateOnly ExamDate { get; set; }
    public Guid? ProviderId { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonIgnore]
    public Patient? Patient { get; set; }
    public Provider? Provider { get; set; }
    [JsonIgnore]
    public ICollection<PerioMeasurement> Measurements { get; set; } = [];
}
