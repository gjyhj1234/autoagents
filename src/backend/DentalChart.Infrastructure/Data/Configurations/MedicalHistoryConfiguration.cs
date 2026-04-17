using DentalChart.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalChart.Infrastructure.Data.Configurations;

public class MedicalHistoryConfiguration : IEntityTypeConfiguration<MedicalHistory>
{
    public void Configure(EntityTypeBuilder<MedicalHistory> builder)
    {
        builder.ToTable("medical_histories");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.PatientId).HasColumnName("patient_id").IsRequired();
        builder.Property(e => e.HasDiabetes).HasColumnName("has_diabetes").HasDefaultValue(false);
        builder.Property(e => e.HasHypertension).HasColumnName("has_hypertension").HasDefaultValue(false);
        builder.Property(e => e.HasHeartDisease).HasColumnName("has_heart_disease").HasDefaultValue(false);
        builder.Property(e => e.HasArtificialHeartValve).HasColumnName("has_artificial_heart_valve").HasDefaultValue(false);
        builder.Property(e => e.HasPacemaker).HasColumnName("has_pacemaker").HasDefaultValue(false);
        builder.Property(e => e.HasBloodThinners).HasColumnName("has_blood_thinners").HasDefaultValue(false);
        builder.Property(e => e.HasBisphosphonates).HasColumnName("has_bisphosphonates").HasDefaultValue(false);
        builder.Property(e => e.HasBleedingDisorder).HasColumnName("has_bleeding_disorder").HasDefaultValue(false);
        builder.Property(e => e.HasHiv).HasColumnName("has_hiv").HasDefaultValue(false);
        builder.Property(e => e.HasHepatitis).HasColumnName("has_hepatitis").HasDefaultValue(false);
        builder.Property(e => e.HasEpilepsy).HasColumnName("has_epilepsy").HasDefaultValue(false);
        builder.Property(e => e.HasAsthma).HasColumnName("has_asthma").HasDefaultValue(false);
        builder.Property(e => e.IsPregnant).HasColumnName("is_pregnant").HasDefaultValue(false);
        builder.Property(e => e.IsNursing).HasColumnName("is_nursing").HasDefaultValue(false);
        builder.Property(e => e.OtherConditions).HasColumnName("other_conditions");
        builder.Property(e => e.CurrentMedications).HasColumnName("current_medications");
        builder.Property(e => e.Allergies).HasColumnName("allergies").HasColumnType("jsonb").HasDefaultValueSql("'[]'");
        builder.Property(e => e.AlertFlags).HasColumnName("alert_flags").HasColumnType("jsonb").HasDefaultValueSql("'[]'");
        builder.Property(e => e.LastUpdated).HasColumnName("last_updated").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
    }
}
