using DentalChart.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalChart.Infrastructure.Data.Configurations;

public class ProcedureCodeConfiguration : IEntityTypeConfiguration<ProcedureCode>
{
    public void Configure(EntityTypeBuilder<ProcedureCode> builder)
    {
        builder.ToTable("procedure_codes");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.AdaCode).HasColumnName("ada_code").HasMaxLength(10).IsRequired();
        builder.Property(e => e.Description).HasColumnName("description").IsRequired();
        builder.Property(e => e.AbbreviatedDesc).HasColumnName("abbreviated_desc").HasMaxLength(50);
        builder.Property(e => e.Category).HasColumnName("category").HasMaxLength(100);
        builder.Property(e => e.DefaultFee).HasColumnName("default_fee").HasColumnType("decimal(10,2)");
        builder.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.HasIndex(e => e.AdaCode).IsUnique();
    }
}
