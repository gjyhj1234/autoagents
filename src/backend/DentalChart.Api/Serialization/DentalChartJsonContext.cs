using System.Text.Json.Serialization;
using DentalChart.Api.Contracts;
using DentalChart.Api.Contracts.Auth;
using DentalChart.Api.Contracts.Patients;

namespace DentalChart.Api.Serialization;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(HealthResponse))]
[JsonSerializable(typeof(ErrorResponse))]
// Auth
[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(LoginResponse))]
[JsonSerializable(typeof(UserDto))]
[JsonSerializable(typeof(RefreshRequest))]
[JsonSerializable(typeof(RefreshResponse))]
[JsonSerializable(typeof(ChangePasswordRequest))]
// Patients
[JsonSerializable(typeof(PatientDto))]
[JsonSerializable(typeof(PatientSummaryDto))]
[JsonSerializable(typeof(CreatePatientRequest))]
[JsonSerializable(typeof(UpdatePatientRequest))]
[JsonSerializable(typeof(PagedResponse<PatientSummaryDto>))]
[JsonSerializable(typeof(List<PatientSummaryDto>))]
[JsonSerializable(typeof(AddressDto))]
[JsonSerializable(typeof(ProviderRefDto))]
// Medical History
[JsonSerializable(typeof(MedicalHistoryDto))]
[JsonSerializable(typeof(UpsertMedicalHistoryRequest))]
[JsonSerializable(typeof(AllergyDto))]
[JsonSerializable(typeof(AlertFlagDto))]
[JsonSerializable(typeof(List<AllergyDto>))]
[JsonSerializable(typeof(List<AlertFlagDto>))]
// Insurance
[JsonSerializable(typeof(InsurancePlanDto))]
[JsonSerializable(typeof(CreateInsurancePlanRequest))]
[JsonSerializable(typeof(UpdateInsurancePlanRequest))]
[JsonSerializable(typeof(List<InsurancePlanDto>))]
// Pagination
[JsonSerializable(typeof(PaginationMeta))]
internal sealed partial class DentalChartJsonContext : JsonSerializerContext
{
}
