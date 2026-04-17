namespace DentalChart.Api.Contracts.Patients;

public sealed record PatientDto(
    Guid Id,
    int PatientNumber,
    string FirstName,
    string LastName,
    string? PreferredName,
    string DateOfBirth,
    int Age,
    string? Gender,
    string? PhoneHome,
    string? PhoneMobile,
    string? PhoneWork,
    string? Email,
    AddressDto? Address,
    string Status,
    ProviderRefDto? PreferredProvider,
    MedicalHistoryDto? MedicalHistory,
    IReadOnlyList<InsurancePlanDto>? InsurancePlans,
    IReadOnlyList<AlertFlagDto>? AlertFlags,
    DateTimeOffset CreatedAt);

public sealed record AddressDto(string? Line1, string? Line2, string? City, string? State, string? PostalCode, string? Country);
public sealed record ProviderRefDto(Guid Id, string Name);
