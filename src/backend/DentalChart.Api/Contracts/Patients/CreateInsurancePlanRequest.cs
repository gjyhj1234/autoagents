namespace DentalChart.Api.Contracts.Patients;

public sealed record CreateInsurancePlanRequest(
    string CarrierName,
    string MemberId,
    int Priority = 1,
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
