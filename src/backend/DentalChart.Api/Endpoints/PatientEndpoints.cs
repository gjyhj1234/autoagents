using System.Text.Json;
using DentalChart.Api.Contracts;
using DentalChart.Api.Contracts.Patients;
using DentalChart.Api.Serialization;
using DentalChart.Core.Entities;
using DentalChart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DentalChart.Api.Endpoints;

public static class PatientEndpoints
{
    public static IEndpointRouteBuilder MapPatientEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/patients").RequireAuthorization();

        group.MapGet("/", GetPatientsAsync);
        group.MapPost("/", CreatePatientAsync);
        group.MapGet("/{id:guid}", GetPatientAsync);
        group.MapPut("/{id:guid}", UpdatePatientAsync);
        group.MapDelete("/{id:guid}", DeletePatientAsync);

        // Medical history
        group.MapGet("/{id:guid}/medical-history", GetMedicalHistoryAsync);
        group.MapPut("/{id:guid}/medical-history", UpsertMedicalHistoryAsync);

        // Insurance
        group.MapGet("/{id:guid}/insurance", GetInsuranceAsync);
        group.MapPost("/{id:guid}/insurance", CreateInsuranceAsync);
        group.MapPut("/{id:guid}/insurance/{planId:guid}", UpdateInsuranceAsync);
        group.MapDelete("/{id:guid}/insurance/{planId:guid}", DeleteInsuranceAsync);

        return routes;
    }

    private static async Task<IResult> GetPatientsAsync(
        DentalChartDbContext db,
        int page = 1,
        int pageSize = 20,
        string? search = null,
        string? status = null,
        Guid? provider = null,
        string sortBy = "lastName",
        string sortDir = "asc")
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var query = db.Patients.Where(p => p.DeletedAt == null);

        {
            var s = search.ToLower();
            query = query.Where(p =>
                p.FirstName.ToLower().Contains(s) ||
                p.LastName.ToLower().Contains(s) ||
                (p.PreferredName != null && p.PreferredName.ToLower().Contains(s)) ||
                (p.PhoneMobile != null && p.PhoneMobile.Contains(s)) ||
                (p.PhoneHome != null && p.PhoneHome.Contains(s)) ||
                (p.Email != null && p.Email.ToLower().Contains(s)));
        }

            query = query.Where(p => p.Status == status);

        if (provider.HasValue)
            query = query.Where(p => p.PreferredProviderId == provider.Value);

        query = (sortBy.ToLower(), sortDir.ToLower()) switch
        {
            ("firstname", "desc") => query.OrderByDescending(p => p.FirstName),
            ("firstname", _) => query.OrderBy(p => p.FirstName),
            ("patientnumber", "desc") => query.OrderByDescending(p => p.PatientNumber),
            ("patientnumber", _) => query.OrderBy(p => p.PatientNumber),
            ("createdat", "desc") => query.OrderByDescending(p => p.CreatedAt),
            ("createdat", _) => query.OrderBy(p => p.CreatedAt),
            (_, "desc") => query.OrderByDescending(p => p.LastName),
            _ => query.OrderBy(p => p.LastName),
        };

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var patients = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = patients.Select(MapToSummaryDto).ToList();
        return TypedResults.Ok(new PagedResponse<PatientSummaryDto>(dtos, new PaginationMeta(page, pageSize, totalCount, totalPages)));
    }

    private static async Task<IResult> GetPatientAsync(Guid id, DentalChartDbContext db)
    {
        var patient = await db.Patients
            .Include(p => p.PreferredProvider)
            .Include(p => p.MedicalHistory)
            .Include(p => p.InsurancePlans.Where(i => i.DeletedAt == null))
            .Where(p => p.Id == id && p.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (patient is null)
            return TypedResults.NotFound(new ErrorResponse("PatientNotFound", "Patient not found"));

        return TypedResults.Ok(MapToDto(patient));
    }

    private static async Task<IResult> CreatePatientAsync(
        CreatePatientRequest request,
        DentalChartDbContext db)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName))
            return TypedResults.BadRequest(new ErrorResponse("ValidationError", "FirstName is required"));
        if (string.IsNullOrWhiteSpace(request.LastName))
            return TypedResults.BadRequest(new ErrorResponse("ValidationError", "LastName is required"));
            return TypedResults.BadRequest(new ErrorResponse("ValidationError", "DateOfBirth must be a valid date (yyyy-MM-dd)"));

        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = dob,
            PreferredName = request.PreferredName,
            Gender = request.Gender,
            PhoneHome = request.PhoneHome,
            PhoneMobile = request.PhoneMobile,
            PhoneWork = request.PhoneWork,
            Email = request.Email,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Country = request.Country ?? "CN",
            PreferredProviderId = request.PreferredProviderId,
            Notes = request.Notes,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        db.Patients.Add(patient);
        await db.SaveChangesAsync();

        return TypedResults.Created($"/api/patients/{patient.Id}", MapToSummaryDto(patient));
    }

    private static async Task<IResult> UpdatePatientAsync(
        Guid id,
        UpdatePatientRequest request,
        DentalChartDbContext db)
    {
        var patient = await db.Patients
            .Where(p => p.Id == id && p.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (patient is null)
            return TypedResults.NotFound(new ErrorResponse("PatientNotFound", "Patient not found"));

        if (request.FirstName is not null) patient.FirstName = request.FirstName;
        if (request.LastName is not null) patient.LastName = request.LastName;
        if (request.DateOfBirth is not null)
        {
                return TypedResults.BadRequest(new ErrorResponse("ValidationError", "DateOfBirth must be a valid date"));
            patient.DateOfBirth = dob;
        }
        if (request.PreferredName is not null) patient.PreferredName = request.PreferredName;
        if (request.Gender is not null) patient.Gender = request.Gender;
        if (request.PhoneHome is not null) patient.PhoneHome = request.PhoneHome;
        if (request.PhoneMobile is not null) patient.PhoneMobile = request.PhoneMobile;
        if (request.PhoneWork is not null) patient.PhoneWork = request.PhoneWork;
        if (request.Email is not null) patient.Email = request.Email;
        if (request.AddressLine1 is not null) patient.AddressLine1 = request.AddressLine1;
        if (request.AddressLine2 is not null) patient.AddressLine2 = request.AddressLine2;
        if (request.City is not null) patient.City = request.City;
        if (request.State is not null) patient.State = request.State;
        if (request.PostalCode is not null) patient.PostalCode = request.PostalCode;
        if (request.Country is not null) patient.Country = request.Country;
        if (request.Status is not null) patient.Status = request.Status;
        if (request.Notes is not null) patient.Notes = request.Notes;
        if (request.PreferredProviderId.HasValue) patient.PreferredProviderId = request.PreferredProviderId.Value;
        patient.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync();

        return TypedResults.Ok(MapToSummaryDto(patient));
    }

    private static async Task<IResult> DeletePatientAsync(Guid id, DentalChartDbContext db)
    {
        var patient = await db.Patients
            .Where(p => p.Id == id && p.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (patient is null)
            return TypedResults.NotFound(new ErrorResponse("PatientNotFound", "Patient not found"));

        patient.DeletedAt = DateTimeOffset.UtcNow;
        patient.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    // Medical History
    private static async Task<IResult> GetMedicalHistoryAsync(Guid id, DentalChartDbContext db)
    {
        var patient = await db.Patients.Where(p => p.Id == id && p.DeletedAt == null).FirstOrDefaultAsync();
        if (patient is null)
            return TypedResults.NotFound(new ErrorResponse("PatientNotFound", "Patient not found"));

        var history = await db.MedicalHistories.Where(m => m.PatientId == id).FirstOrDefaultAsync();
        if (history is null)
            return TypedResults.NotFound(new ErrorResponse("MedicalHistoryNotFound", "Medical history not found"));

        return TypedResults.Ok(MapToMedicalHistoryDto(history));
    }

    private static async Task<IResult> UpsertMedicalHistoryAsync(
        Guid id,
        UpsertMedicalHistoryRequest request,
        DentalChartDbContext db)
    {
        var patient = await db.Patients.Where(p => p.Id == id && p.DeletedAt == null).FirstOrDefaultAsync();
        if (patient is null)
            return TypedResults.NotFound(new ErrorResponse("PatientNotFound", "Patient not found"));

        var history = await db.MedicalHistories.Where(m => m.PatientId == id).FirstOrDefaultAsync();
        var now = DateTimeOffset.UtcNow;

        var allergiesJson = JsonSerializer.Serialize(request.Allergies ?? [], DentalChartJsonContext.Default.ListAllergyDto);
        var alertFlagsJson = JsonSerializer.Serialize(request.AlertFlags ?? [], DentalChartJsonContext.Default.ListAlertFlagDto);

        if (history is null)
        {
            history = new MedicalHistory
            {
                Id = Guid.NewGuid(),
                PatientId = id,
                CreatedAt = now,
            };
            db.MedicalHistories.Add(history);
        }

        history.HasDiabetes = request.HasDiabetes;
        history.HasHypertension = request.HasHypertension;
        history.HasHeartDisease = request.HasHeartDisease;
        history.HasArtificialHeartValve = request.HasArtificialHeartValve;
        history.HasPacemaker = request.HasPacemaker;
        history.HasBloodThinners = request.HasBloodThinners;
        history.HasBisphosphonates = request.HasBisphosphonates;
        history.HasBleedingDisorder = request.HasBleedingDisorder;
        history.HasHiv = request.HasHiv;
        history.HasHepatitis = request.HasHepatitis;
        history.HasEpilepsy = request.HasEpilepsy;
        history.HasAsthma = request.HasAsthma;
        history.IsPregnant = request.IsPregnant;
        history.IsNursing = request.IsNursing;
        history.OtherConditions = request.OtherConditions;
        history.CurrentMedications = request.CurrentMedications;
        history.Allergies = allergiesJson;
        history.AlertFlags = alertFlagsJson;
        history.LastUpdated = now;
        history.UpdatedAt = now;

        await db.SaveChangesAsync();
        return TypedResults.Ok(MapToMedicalHistoryDto(history));
    }

    // Insurance
    private static async Task<IResult> GetInsuranceAsync(Guid id, DentalChartDbContext db)
    {
        var patient = await db.Patients.Where(p => p.Id == id && p.DeletedAt == null).FirstOrDefaultAsync();
        if (patient is null)
            return TypedResults.NotFound(new ErrorResponse("PatientNotFound", "Patient not found"));

        var plans = await db.InsurancePlans
            .Where(i => i.PatientId == id && i.DeletedAt == null)
            .OrderBy(i => i.Priority)
            .ToListAsync();

        return TypedResults.Ok(plans.Select(MapToInsurancePlanDto).ToList());
    }

    private static async Task<IResult> CreateInsuranceAsync(
        Guid id,
        CreateInsurancePlanRequest request,
        DentalChartDbContext db)
    {
        var patient = await db.Patients.Where(p => p.Id == id && p.DeletedAt == null).FirstOrDefaultAsync();
        if (patient is null)
            return TypedResults.NotFound(new ErrorResponse("PatientNotFound", "Patient not found"));

        var plan = new InsurancePlan
        {
            Id = Guid.NewGuid(),
            PatientId = id,
            CarrierName = request.CarrierName,
            MemberId = request.MemberId,
            Priority = request.Priority,
            PlanName = request.PlanName,
            GroupNumber = request.GroupNumber,
            EmployerName = request.EmployerName,
            SubscriberName = request.SubscriberName,
            SubscriberDob = request.SubscriberDob is not null && DateOnly.TryParse(request.SubscriberDob, out var sdob) ? sdob : null,
            SubscriberRelationship = request.SubscriberRelationship,
            EffectiveDate = request.EffectiveDate is not null && DateOnly.TryParse(request.EffectiveDate, out var edate) ? edate : null,
            TerminationDate = request.TerminationDate is not null && DateOnly.TryParse(request.TerminationDate, out var tdate) ? tdate : null,
            AnnualMaximum = request.AnnualMaximum,
            Deductible = request.Deductible,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        db.InsurancePlans.Add(plan);
        await db.SaveChangesAsync();

        return TypedResults.Created($"/api/patients/{id}/insurance/{plan.Id}", MapToInsurancePlanDto(plan));
    }

    private static async Task<IResult> UpdateInsuranceAsync(
        Guid id,
        Guid planId,
        UpdateInsurancePlanRequest request,
        DentalChartDbContext db)
    {
        var plan = await db.InsurancePlans
            .Where(i => i.Id == planId && i.PatientId == id && i.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (plan is null)
            return TypedResults.NotFound(new ErrorResponse("InsurancePlanNotFound", "Insurance plan not found"));

        if (request.CarrierName is not null) plan.CarrierName = request.CarrierName;
        if (request.MemberId is not null) plan.MemberId = request.MemberId;
        if (request.Priority.HasValue) plan.Priority = request.Priority.Value;
        if (request.PlanName is not null) plan.PlanName = request.PlanName;
        if (request.GroupNumber is not null) plan.GroupNumber = request.GroupNumber;
        if (request.EmployerName is not null) plan.EmployerName = request.EmployerName;
        if (request.SubscriberName is not null) plan.SubscriberName = request.SubscriberName;
        if (request.SubscriberDob is not null)
            plan.SubscriberDob = DateOnly.TryParse(request.SubscriberDob, out var sdob) ? sdob : plan.SubscriberDob;
        if (request.SubscriberRelationship is not null) plan.SubscriberRelationship = request.SubscriberRelationship;
        if (request.EffectiveDate is not null)
            plan.EffectiveDate = DateOnly.TryParse(request.EffectiveDate, out var edate) ? edate : plan.EffectiveDate;
        if (request.TerminationDate is not null)
            plan.TerminationDate = DateOnly.TryParse(request.TerminationDate, out var tdate) ? tdate : plan.TerminationDate;
        if (request.AnnualMaximum.HasValue) plan.AnnualMaximum = request.AnnualMaximum.Value;
        if (request.Deductible.HasValue) plan.Deductible = request.Deductible.Value;
        plan.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync();
        return TypedResults.Ok(MapToInsurancePlanDto(plan));
    }

    private static async Task<IResult> DeleteInsuranceAsync(
        Guid id,
        Guid planId,
        DentalChartDbContext db)
    {
        var plan = await db.InsurancePlans
            .Where(i => i.Id == planId && i.PatientId == id && i.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (plan is null)
            return TypedResults.NotFound(new ErrorResponse("InsurancePlanNotFound", "Insurance plan not found"));

        plan.DeletedAt = DateTimeOffset.UtcNow;
        plan.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    // Mapping helpers
    private static PatientSummaryDto MapToSummaryDto(Patient p) => new(
        p.Id,
        p.PatientNumber,
        p.FirstName,
        p.LastName,
        p.PreferredName,
        p.DateOfBirth.ToString("yyyy-MM-dd"),
        CalculateAge(p.DateOfBirth),
        p.Gender,
        p.PhoneMobile,
        p.Email,
        p.Status,
        p.CreatedAt);

    private static PatientDto MapToDto(Patient p)
    {
        var alertFlags = new List<AlertFlagDto>();
        if (p.MedicalHistory is not null)
        {
            var flags = TryDeserializeAlertFlags(p.MedicalHistory.AlertFlags);
            alertFlags.AddRange(flags);
        }

        return new PatientDto(
            p.Id,
            p.PatientNumber,
            p.FirstName,
            p.LastName,
            p.PreferredName,
            p.DateOfBirth.ToString("yyyy-MM-dd"),
            CalculateAge(p.DateOfBirth),
            p.Gender,
            p.PhoneHome,
            p.PhoneMobile,
            p.PhoneWork,
            p.Email,
            new AddressDto(p.AddressLine1, p.AddressLine2, p.City, p.State, p.PostalCode, p.Country),
            p.Status,
            p.PreferredProvider is not null
                ? new ProviderRefDto(p.PreferredProvider.Id, $"{p.PreferredProvider.FirstName} {p.PreferredProvider.LastName}")
                : null,
            p.MedicalHistory is not null ? MapToMedicalHistoryDto(p.MedicalHistory) : null,
            p.InsurancePlans.Select(MapToInsurancePlanDto).ToList(),
            alertFlags,
            p.CreatedAt);
    }

    private static MedicalHistoryDto MapToMedicalHistoryDto(MedicalHistory h) => new(
        h.HasDiabetes,
        h.HasHypertension,
        h.HasHeartDisease,
        h.HasArtificialHeartValve,
        h.HasPacemaker,
        h.HasBloodThinners,
        h.HasBisphosphonates,
        h.HasBleedingDisorder,
        h.HasHiv,
        h.HasHepatitis,
        h.HasEpilepsy,
        h.HasAsthma,
        h.IsPregnant,
        h.IsNursing,
        h.OtherConditions,
        h.CurrentMedications,
        TryDeserializeAllergies(h.Allergies),
        TryDeserializeAlertFlags(h.AlertFlags),
        h.LastUpdated);

    private static InsurancePlanDto MapToInsurancePlanDto(InsurancePlan i) => new(
        i.Id,
        i.Priority,
        i.CarrierName,
        i.PlanName,
        i.GroupNumber,
        i.MemberId,
        i.EmployerName,
        i.SubscriberName,
        i.SubscriberDob?.ToString("yyyy-MM-dd"),
        i.SubscriberRelationship,
        i.EffectiveDate?.ToString("yyyy-MM-dd"),
        i.TerminationDate?.ToString("yyyy-MM-dd"),
        i.AnnualMaximum,
        i.Deductible);

    private static int CalculateAge(DateOnly dob)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dob.Year;
        if (dob > today.AddYears(-age)) age--;
        return age;
    }

    private static IReadOnlyList<AllergyDto> TryDeserializeAllergies(string json)
    {
        try
        {
            return JsonSerializer.Deserialize(json, DentalChartJsonContext.Default.ListAllergyDto) ?? [];
        }
        catch { return []; }
    }

    private static IReadOnlyList<AlertFlagDto> TryDeserializeAlertFlags(string json)
    {
        try
        {
            return JsonSerializer.Deserialize(json, DentalChartJsonContext.Default.ListAlertFlagDto) ?? [];
        }
        catch { return []; }
    }
}
