using DentalChart.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalChart.Infrastructure.Data.Configurations;

public class InsurancePlanConfiguration : IEntityTypeConfiguration<InsurancePlan>
{
    public void Configure(EntityTypeBuilder<InsurancePlan> builder)
    {
        builder.ToTable("insurance_plans");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.PatientId).HasColumnName("patient_id").IsRequired();
        builder.Property(e => e.Priority).HasColumnName("priority").HasDefaultValue(1);
        builder.Property(e => e.CarrierName).HasColumnName("carrier_name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.PlanName).HasColumnName("plan_name").HasMaxLength(200);
        builder.Property(e => e.GroupNumber).HasColumnName("group_number").HasMaxLength(50);
        builder.Property(e => e.MemberId).HasColumnName("member_id").HasMaxLength(50).IsRequired();
        builder.Property(e => e.EmployerName).HasColumnName("employer_name").HasMaxLength(200);
        builder.Property(e => e.SubscriberName).HasColumnName("subscriber_name").HasMaxLength(200);
        builder.Property(e => e.SubscriberDob).HasColumnName("subscriber_dob");
        builder.Property(e => e.SubscriberRelationship).HasColumnName("subscriber_relationship").HasMaxLength(50);
        builder.Property(e => e.EffectiveDate).HasColumnName("effective_date");
        builder.Property(e => e.TerminationDate).HasColumnName("termination_date");
        builder.Property(e => e.AnnualMaximum).HasColumnName("annual_maximum").HasColumnType("decimal(10,2)");
        builder.Property(e => e.Deductible).HasColumnName("deductible").HasColumnType("decimal(10,2)");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.DeletedAt).HasColumnName("deleted_at");
    }
}
