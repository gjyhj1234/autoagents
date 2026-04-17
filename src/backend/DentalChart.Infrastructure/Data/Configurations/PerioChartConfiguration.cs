using DentalChart.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalChart.Infrastructure.Data.Configurations;

public class PerioChartConfiguration : IEntityTypeConfiguration<PerioChart>
{
    public void Configure(EntityTypeBuilder<PerioChart> builder)
    {
        builder.ToTable("perio_charts");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.PatientId).HasColumnName("patient_id").IsRequired();
        builder.Property(e => e.ExamDate).HasColumnName("exam_date").IsRequired();
        builder.Property(e => e.ProviderId).HasColumnName("provider_id");
        builder.Property(e => e.Notes).HasColumnName("notes");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.HasOne(e => e.Provider).WithMany(p => p.PerioCharts).HasForeignKey(e => e.ProviderId);
        builder.HasMany(e => e.Measurements).WithOne(m => m.Chart).HasForeignKey(m => m.ChartId);
    }
}
