using DentalChart.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalChart.Infrastructure.Data.Configurations;

public class TreatmentPlanConfiguration : IEntityTypeConfiguration<TreatmentPlan>
{
    public void Configure(EntityTypeBuilder<TreatmentPlan> builder)
    {
        builder.ToTable("treatment_plans");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.PatientId).HasColumnName("patient_id").IsRequired();
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(200).HasDefaultValue("Treatment Plan");
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("active");
        builder.Property(e => e.AcceptedDate).HasColumnName("accepted_date");
        builder.Property(e => e.AcceptedSignature).HasColumnName("accepted_signature");
        builder.Property(e => e.Notes).HasColumnName("notes");
        builder.Property(e => e.CreatedBy).HasColumnName("created_by");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.DeletedAt).HasColumnName("deleted_at");
        builder.HasMany(e => e.Items).WithOne(i => i.Plan).HasForeignKey(i => i.PlanId);
    }
}
