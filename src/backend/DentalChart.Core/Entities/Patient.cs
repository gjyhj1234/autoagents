using System.Text.Json.Serialization;

namespace DentalChart.Core.Entities;

public class Patient
{
    public Guid Id { get; set; }
    public int PatientNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PreferredName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? PhoneHome { get; set; }
    public string? PhoneMobile { get; set; }
    public string? PhoneWork { get; set; }
    public string? Email { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string Country { get; set; } = "CN";
    public string? SsnLast4 { get; set; }
    public Guid? PreferredProviderId { get; set; }
    public string PreferredLanguage { get; set; } = "zh-CN";
    public string Status { get; set; } = "active";
    public string? ReferralSource { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public Provider? PreferredProvider { get; set; }

    [JsonIgnore]
    public MedicalHistory? MedicalHistory { get; set; }
    [JsonIgnore]
    public ICollection<InsurancePlan> InsurancePlans { get; set; } = [];
    [JsonIgnore]
    public ICollection<PatientChart> DentalCharts { get; set; } = [];
    [JsonIgnore]
    public ICollection<TreatmentPlan> TreatmentPlans { get; set; } = [];
    [JsonIgnore]
    public ICollection<PerioChart> PerioCharts { get; set; } = [];
    [JsonIgnore]
    public ICollection<Appointment> Appointments { get; set; } = [];
}
