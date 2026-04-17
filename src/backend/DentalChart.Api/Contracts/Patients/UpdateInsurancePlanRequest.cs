namespace DentalChart.Api.Contracts.Patients;

public sealed record UpdateInsurancePlanRequest(
    string? CarrierName = null,
    string? MemberId = null,
    int? Priority = null,
    string? PlanName = null,
    string? GroupNumber = null,
    string? EmployerName = null,
    string? SubscriberName = null,
    string? SubscriberDob = null,
    string? SubscriberRelationship = null,
    string? EffectiveDate = null,
    string? TerminationDate = null,
    decimal? AnnualMaximum = null,
    decimal? Deductible = null);
