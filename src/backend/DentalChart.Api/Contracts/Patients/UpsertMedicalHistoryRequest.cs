namespace DentalChart.Api.Contracts.Patients;

public sealed record UpsertMedicalHistoryRequest(
    bool HasDiabetes = false,
    bool HasHypertension = false,
    bool HasHeartDisease = false,
    bool HasArtificialHeartValve = false,
    bool HasPacemaker = false,
    bool HasBloodThinners = false,
    bool HasBisphosphonates = false,
    bool HasBleedingDisorder = false,
    bool HasHiv = false,
    bool HasHepatitis = false,
    bool HasEpilepsy = false,
    bool HasAsthma = false,
    bool IsPregnant = false,
    bool IsNursing = false,
    string? OtherConditions = null,
    string? CurrentMedications = null,
    IReadOnlyList<AllergyDto>? Allergies = null,
    IReadOnlyList<AlertFlagDto>? AlertFlags = null);
