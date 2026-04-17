using DentalChart.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DentalChart.Infrastructure.Data;

public sealed class DentalChartDbContext(DbContextOptions<DentalChartDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<ProcedureCode> ProcedureCodes => Set<ProcedureCode>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<MedicalHistory> MedicalHistories => Set<MedicalHistory>();
    public DbSet<InsurancePlan> InsurancePlans => Set<InsurancePlan>();
    public DbSet<PatientChart> DentalCharts => Set<PatientChart>();
    public DbSet<ToothCondition> ToothConditions => Set<ToothCondition>();
    public DbSet<TreatmentPlan> TreatmentPlans => Set<TreatmentPlan>();
    public DbSet<TreatmentPlanItem> TreatmentPlanItems => Set<TreatmentPlanItem>();
    public DbSet<PerioChart> PerioCharts => Set<PerioChart>();
    public DbSet<PerioMeasurement> PerioMeasurements => Set<PerioMeasurement>();
    public DbSet<Operatory> Operatories => Set<Operatory>();
    public DbSet<AppointmentType> AppointmentTypes => Set<AppointmentType>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DentalChartDbContext).Assembly);
    }
}

