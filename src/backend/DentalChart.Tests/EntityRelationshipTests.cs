using DentalChart.Core.Entities;

namespace DentalChart.Tests;

public sealed class EntityRelationshipTests
{
    [Fact]
    public void Patient_HasCorrectDefaultValues()
    {
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "Patient",
            DateOfBirth = new DateOnly(1990, 1, 1),
        };

        Assert.Equal("active", patient.Status);
        Assert.Equal("CN", patient.Country);
        Assert.Equal("zh-CN", patient.PreferredLanguage);
        Assert.NotNull(patient.InsurancePlans);
        Assert.NotNull(patient.DentalCharts);
        Assert.NotNull(patient.TreatmentPlans);
        Assert.NotNull(patient.PerioCharts);
        Assert.NotNull(patient.Appointments);
    }

    [Fact]
    public void TreatmentPlan_HasCorrectDefaultValues()
    {
        var plan = new TreatmentPlan
        {
            Id = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
        };

        Assert.Equal("Treatment Plan", plan.Name);
        Assert.Equal("active", plan.Status);
        Assert.NotNull(plan.Items);
    }

    [Fact]
    public void TreatmentPlanItem_HasCorrectDefaultValues()
    {
        var item = new TreatmentPlanItem
        {
            Id = Guid.NewGuid(),
            PlanId = Guid.NewGuid(),
            ProcedureCodeId = Guid.NewGuid(),
        };

        Assert.Equal("planned", item.Status);
        Assert.Equal(1, item.VisitNumber);
        Assert.Equal(0, item.SortOrder);
    }

    [Fact]
    public void Provider_HasCorrectDefaultValues()
    {
        var provider = new Provider
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
        };

        Assert.Equal("#3182CE", provider.Color);
        Assert.True(provider.IsActive);
        Assert.NotNull(provider.Appointments);
        Assert.NotNull(provider.ToothConditions);
        Assert.NotNull(provider.TreatmentPlanItems);
        Assert.NotNull(provider.PerioCharts);
    }

    [Fact]
    public void MedicalHistory_HasCorrectDefaultValues()
    {
        var history = new MedicalHistory
        {
            Id = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
        };

        Assert.False(history.HasDiabetes);
        Assert.False(history.HasHypertension);
        Assert.False(history.HasHeartDisease);
        Assert.False(history.IsPregnant);
        Assert.Equal("[]", history.Allergies);
        Assert.Equal("[]", history.AlertFlags);
    }

    [Fact]
    public void ProcedureCode_HasCorrectDefaultValues()
    {
        var code = new ProcedureCode
        {
            Id = Guid.NewGuid(),
            AdaCode = "D0120",
            Description = "Periodic oral evaluation",
        };

        Assert.True(code.IsActive);
        Assert.NotNull(code.ToothConditions);
        Assert.NotNull(code.TreatmentPlanItems);
    }

    [Fact]
    public void PatientChart_HasCorrectDefaultValues()
    {
        var chart = new PatientChart
        {
            Id = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
        };

        Assert.Equal("permanent", chart.DentitionMode);
        Assert.Equal("universal", chart.NotationSystem);
        Assert.NotNull(chart.ToothConditions);
    }

    [Fact]
    public void PerioChart_HasNavigationProperties()
    {
        var chart = new PerioChart
        {
            Id = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
            ExamDate = DateOnly.FromDateTime(DateTime.Today),
        };

        Assert.NotNull(chart.Measurements);
    }

    [Fact]
    public void Appointment_HasCorrectDefaultStatus()
    {
        var appt = new Appointment
        {
            Id = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
            ProviderId = Guid.NewGuid(),
            StartTime = DateTimeOffset.UtcNow,
            EndTime = DateTimeOffset.UtcNow.AddHours(1),
        };

        Assert.Equal("scheduled", appt.Status);
    }

    [Fact]
    public void ToothCondition_HasCorrectDefaultStatus()
    {
        var condition = new ToothCondition
        {
            Id = Guid.NewGuid(),
            ChartId = Guid.NewGuid(),
            ToothNumber = "14",
            ConditionType = "decay",
        };

        Assert.Equal("existing_other", condition.Status);
    }

    [Fact]
    public void InsurancePlan_HasCorrectDefaultPriority()
    {
        var plan = new InsurancePlan
        {
            Id = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
            CarrierName = "Test Insurance",
            MemberId = "MBR001",
        };

        Assert.Equal(1, plan.Priority);
    }

    [Fact]
    public void Operatory_HasCorrectDefaultValues()
    {
        var op = new Operatory
        {
            Id = Guid.NewGuid(),
            Name = "Operatory 1",
        };

        Assert.Equal("#718096", op.Color);
        Assert.True(op.IsActive);
        Assert.Equal(0, op.SortOrder);
    }

    [Fact]
    public void AppointmentType_HasCorrectDefaultValues()
    {
        var type = new AppointmentType
        {
            Id = Guid.NewGuid(),
            Name = "Cleaning",
        };

        Assert.Equal(60, type.DefaultDurationMinutes);
        Assert.Equal("#3182CE", type.Color);
        Assert.True(type.IsActive);
    }

    [Fact]
    public void User_HasCorrectDefaultValues()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashed",
            Role = "dentist",
        };

        Assert.True(user.IsActive);
        Assert.Equal(0, user.FailedLoginAttempts);
        Assert.Null(user.LockedUntil);
        Assert.Null(user.DeletedAt);
    }

    [Fact]
    public void Patient_CanAssignPreferredProvider()
    {
        var providerId = Guid.NewGuid();
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Smith",
            DateOfBirth = new DateOnly(1985, 6, 15),
            PreferredProviderId = providerId,
        };

        Assert.Equal(providerId, patient.PreferredProviderId);
    }

    [Fact]
    public void TreatmentPlan_CanHaveMultipleItems()
    {
        var planId = Guid.NewGuid();
        var codeId = Guid.NewGuid();
        var plan = new TreatmentPlan { Id = planId, PatientId = Guid.NewGuid() };

        plan.Items.Add(new TreatmentPlanItem { Id = Guid.NewGuid(), PlanId = planId, ProcedureCodeId = codeId, SortOrder = 0 });
        plan.Items.Add(new TreatmentPlanItem { Id = Guid.NewGuid(), PlanId = planId, ProcedureCodeId = codeId, SortOrder = 1 });

        Assert.Equal(2, plan.Items.Count);
    }

    [Fact]
    public void PatientChart_CanHaveToothConditions()
    {
        var chartId = Guid.NewGuid();
        var chart = new PatientChart { Id = chartId, PatientId = Guid.NewGuid() };

        chart.ToothConditions.Add(new ToothCondition
        {
            Id = Guid.NewGuid(),
            ChartId = chartId,
            ToothNumber = "3",
            ConditionType = "crown",
            Surfaces = ["O", "M", "D"],
        });

        Assert.Single(chart.ToothConditions);
        Assert.Equal(["O", "M", "D"], chart.ToothConditions.First().Surfaces ?? []);
    }
}
