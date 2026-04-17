namespace DentalChart.Api.Contracts.Patients;

public sealed record UpdatePatientRequest(
    string? FirstName = null,
    string? LastName = null,
    string? DateOfBirth = null,
    string? PreferredName = null,
    string? Gender = null,
    string? PhoneHome = null,
    string? PhoneMobile = null,
    string? PhoneWork = null,
    string? Email = null,
    string? AddressLine1 = null,
    string? AddressLine2 = null,
    string? City = null,
    string? State = null,
    string? PostalCode = null,
    string? Country = null,
    Guid? PreferredProviderId = null,
    string? Status = null,
    string? Notes = null);
