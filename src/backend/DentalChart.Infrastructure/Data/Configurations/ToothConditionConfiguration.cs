using DentalChart.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalChart.Infrastructure.Data.Configurations;

public class ToothConditionConfiguration : IEntityTypeConfiguration<ToothCondition>
{
    public void Configure(EntityTypeBuilder<ToothCondition> builder)
    {
        builder.ToTable("tooth_conditions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.ChartId).HasColumnName("chart_id").IsRequired();
        builder.Property(e => e.ToothNumber).HasColumnName("tooth_number").HasMaxLength(5).IsRequired();
        builder.Property(e => e.Surfaces).HasColumnName("surfaces").HasColumnType("varchar(10)[]");
        builder.Property(e => e.ConditionType).HasColumnName("condition_type").HasMaxLength(50).IsRequired();
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(30).HasDefaultValue("existing_other");
        builder.Property(e => e.ProcedureCodeId).HasColumnName("procedure_code_id");
        builder.Property(e => e.ProviderId).HasColumnName("provider_id");
        builder.Property(e => e.Fee).HasColumnName("fee").HasColumnType("decimal(10,2)");
        builder.Property(e => e.Note).HasColumnName("note");
        builder.Property(e => e.DateCompleted).HasColumnName("date_completed");
        builder.Property(e => e.CreatedBy).HasColumnName("created_by");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.DeletedAt).HasColumnName("deleted_at");
        builder.HasOne(e => e.ProcedureCode).WithMany(p => p.ToothConditions).HasForeignKey(e => e.ProcedureCodeId);
        builder.HasOne(e => e.Provider).WithMany(p => p.ToothConditions).HasForeignKey(e => e.ProviderId);
    }
}
