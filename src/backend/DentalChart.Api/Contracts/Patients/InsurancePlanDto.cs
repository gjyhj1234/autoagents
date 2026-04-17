namespace DentalChart.Api.Contracts.Patients;

public sealed record InsurancePlanDto(
    Guid Id,
    int Priority,
    string CarrierName,
    string? PlanName,
    string? GroupNumber,
    string MemberId,
    string? EmployerName,
    string? SubscriberName,
    string? SubscriberDob,
    string? SubscriberRelationship,
    string? EffectiveDate,
    string? TerminationDate,
    decimal? AnnualMaximum,
    decimal? Deductible);
