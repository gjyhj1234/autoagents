namespace DentalChart.Api.Contracts.Patients;

public sealed record MedicalHistoryDto(
    bool HasDiabetes,
    bool HasHypertension,
    bool HasHeartDisease,
    bool HasArtificialHeartValve,
    bool HasPacemaker,
    bool HasBloodThinners,
    bool HasBisphosphonates,
    bool HasBleedingDisorder,
    bool HasHiv,
    bool HasHepatitis,
    bool HasEpilepsy,
    bool HasAsthma,
    bool IsPregnant,
    bool IsNursing,
    string? OtherConditions,
    string? CurrentMedications,
    IReadOnlyList<AllergyDto>? Allergies,
    IReadOnlyList<AlertFlagDto>? AlertFlags,
    DateTimeOffset LastUpdated);

public sealed record AllergyDto(string Name, string? Severity, string? Reaction);
public sealed record AlertFlagDto(string Text, string? Severity);
