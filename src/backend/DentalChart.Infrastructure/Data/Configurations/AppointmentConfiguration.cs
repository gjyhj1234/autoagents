using DentalChart.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalChart.Infrastructure.Data.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.PatientId).HasColumnName("patient_id").IsRequired();
        builder.Property(e => e.ProviderId).HasColumnName("provider_id").IsRequired();
        builder.Property(e => e.OperatoryId).HasColumnName("operatory_id");
        builder.Property(e => e.AppointmentTypeId).HasColumnName("appointment_type_id");
        builder.Property(e => e.StartTime).HasColumnName("start_time").IsRequired();
        builder.Property(e => e.EndTime).HasColumnName("end_time").IsRequired();
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("scheduled");
        builder.Property(e => e.Notes).HasColumnName("notes");
        builder.Property(e => e.CreatedBy).HasColumnName("created_by");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.DeletedAt).HasColumnName("deleted_at");
        builder.HasOne(e => e.Provider).WithMany(p => p.Appointments).HasForeignKey(e => e.ProviderId);
        builder.HasOne(e => e.Operatory).WithMany().HasForeignKey(e => e.OperatoryId);
        builder.HasOne(e => e.AppointmentType).WithMany().HasForeignKey(e => e.AppointmentTypeId);
    }
}
