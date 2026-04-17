using DentalChart.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalChart.Infrastructure.Data.Configurations;

public class PatientChartConfiguration : IEntityTypeConfiguration<PatientChart>
{
    public void Configure(EntityTypeBuilder<PatientChart> builder)
    {
        builder.ToTable("dental_charts");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.PatientId).HasColumnName("patient_id").IsRequired();
        builder.Property(e => e.DentitionMode).HasColumnName("dentition_mode").HasMaxLength(20).HasDefaultValue("permanent");
        builder.Property(e => e.NotationSystem).HasColumnName("notation_system").HasMaxLength(20).HasDefaultValue("universal");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.HasMany(e => e.ToothConditions).WithOne(tc => tc.Chart).HasForeignKey(tc => tc.ChartId);
    }
}
