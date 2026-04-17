using System.Text.Json.Serialization;

namespace DentalChart.Core.Entities;

public class Appointment
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public Guid? OperatoryId { get; set; }
    public Guid? AppointmentTypeId { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public string Status { get; set; } = "scheduled";
    public string? Notes { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    [JsonIgnore]
    public Patient? Patient { get; set; }
    public Provider? Provider { get; set; }
    public Operatory? Operatory { get; set; }
    public AppointmentType? AppointmentType { get; set; }
}
