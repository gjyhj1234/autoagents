using System.Text.Json.Serialization;

namespace DentalChart.Core.Entities;

public class Provider
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? LicenseNumber { get; set; }
    public string? Specialty { get; set; }
    public string Color { get; set; } = "#3182CE";
    public string? Abbreviation { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public User? User { get; set; }

    [JsonIgnore]
    public ICollection<Appointment> Appointments { get; set; } = [];
    [JsonIgnore]
    public ICollection<ToothCondition> ToothConditions { get; set; } = [];
    [JsonIgnore]
    public ICollection<TreatmentPlanItem> TreatmentPlanItems { get; set; } = [];
    [JsonIgnore]
    public ICollection<PerioChart> PerioCharts { get; set; } = [];
}
