using DentalChart.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalChart.Infrastructure.Data.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("patients");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.PatientNumber).HasColumnName("patient_number").ValueGeneratedOnAdd();
        builder.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.PreferredName).HasColumnName("preferred_name").HasMaxLength(100);
        builder.Property(e => e.DateOfBirth).HasColumnName("date_of_birth").IsRequired();
        builder.Property(e => e.Gender).HasColumnName("gender").HasMaxLength(20);
        builder.Property(e => e.PhoneHome).HasColumnName("phone_home").HasMaxLength(20);
        builder.Property(e => e.PhoneMobile).HasColumnName("phone_mobile").HasMaxLength(20);
        builder.Property(e => e.PhoneWork).HasColumnName("phone_work").HasMaxLength(20);
        builder.Property(e => e.Email).HasColumnName("email").HasMaxLength(255);
        builder.Property(e => e.AddressLine1).HasColumnName("address_line1").HasMaxLength(255);
        builder.Property(e => e.AddressLine2).HasColumnName("address_line2").HasMaxLength(255);
        builder.Property(e => e.City).HasColumnName("city").HasMaxLength(100);
        builder.Property(e => e.State).HasColumnName("state").HasMaxLength(50);
        builder.Property(e => e.PostalCode).HasColumnName("postal_code").HasMaxLength(20);
        builder.Property(e => e.Country).HasColumnName("country").HasMaxLength(50).HasDefaultValue("CN");
        builder.Property(e => e.SsnLast4).HasColumnName("ssn_last4").HasMaxLength(4);
        builder.Property(e => e.PreferredProviderId).HasColumnName("preferred_provider_id");
        builder.Property(e => e.PreferredLanguage).HasColumnName("preferred_language").HasMaxLength(10).HasDefaultValue("zh-CN");
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("active");
        builder.Property(e => e.ReferralSource).HasColumnName("referral_source").HasMaxLength(100);
        builder.Property(e => e.Notes).HasColumnName("notes");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.DeletedAt).HasColumnName("deleted_at");
        builder.HasOne(e => e.PreferredProvider).WithMany().HasForeignKey(e => e.PreferredProviderId);
        builder.HasOne(e => e.MedicalHistory).WithOne(m => m.Patient).HasForeignKey<MedicalHistory>(m => m.PatientId);
        builder.HasMany(e => e.InsurancePlans).WithOne(i => i.Patient).HasForeignKey(i => i.PatientId);
        builder.HasMany(e => e.DentalCharts).WithOne(dc => dc.Patient).HasForeignKey(dc => dc.PatientId);
        builder.HasMany(e => e.TreatmentPlans).WithOne(tp => tp.Patient).HasForeignKey(tp => tp.PatientId);
        builder.HasMany(e => e.PerioCharts).WithOne(pc => pc.Patient).HasForeignKey(pc => pc.PatientId);
        builder.HasMany(e => e.Appointments).WithOne(a => a.Patient).HasForeignKey(a => a.PatientId);
    }
}
