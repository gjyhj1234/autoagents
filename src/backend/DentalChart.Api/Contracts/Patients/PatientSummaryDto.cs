namespace DentalChart.Api.Contracts.Patients;

public sealed record PatientSummaryDto(
    Guid Id,
    int PatientNumber,
    string FirstName,
    string LastName,
    string? PreferredName,
    string DateOfBirth,
    int Age,
    string? Gender,
    string? PhoneMobile,
    string? Email,
    string Status,
    DateTimeOffset CreatedAt);
