using DentalChart.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalChart.Infrastructure.Data.Configurations;

public class TreatmentPlanItemConfiguration : IEntityTypeConfiguration<TreatmentPlanItem>
{
    public void Configure(EntityTypeBuilder<TreatmentPlanItem> builder)
    {
        builder.ToTable("treatment_plan_items");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.PlanId).HasColumnName("plan_id").IsRequired();
        builder.Property(e => e.ToothNumber).HasColumnName("tooth_number").HasMaxLength(5);
        builder.Property(e => e.Surfaces).HasColumnName("surfaces").HasColumnType("varchar(10)[]");
        builder.Property(e => e.ProcedureCodeId).HasColumnName("procedure_code_id").IsRequired();
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(30).HasDefaultValue("planned");
        builder.Property(e => e.ProviderId).HasColumnName("provider_id");
        builder.Property(e => e.Fee).HasColumnName("fee").HasColumnType("decimal(10,2)");
        builder.Property(e => e.InsuranceEstimate).HasColumnName("insurance_estimate").HasColumnType("decimal(10,2)");
        builder.Property(e => e.PatientPortion).HasColumnName("patient_portion").HasColumnType("decimal(10,2)");
        builder.Property(e => e.VisitNumber).HasColumnName("visit_number").HasDefaultValue(1);
        builder.Property(e => e.SortOrder).HasColumnName("sort_order").HasDefaultValue(0);
        builder.Property(e => e.Note).HasColumnName("note");
        builder.Property(e => e.AppointmentId).HasColumnName("appointment_id");
        builder.Property(e => e.DateCompleted).HasColumnName("date_completed");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.DeletedAt).HasColumnName("deleted_at");
        builder.HasOne(e => e.ProcedureCode).WithMany(p => p.TreatmentPlanItems).HasForeignKey(e => e.ProcedureCodeId);
        builder.HasOne(e => e.Provider).WithMany(p => p.TreatmentPlanItems).HasForeignKey(e => e.ProviderId);
    }
}
