using DentalChart.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalChart.Infrastructure.Data.Configurations;

public class PerioMeasurementConfiguration : IEntityTypeConfiguration<PerioMeasurement>
{
    public void Configure(EntityTypeBuilder<PerioMeasurement> builder)
    {
        builder.ToTable("perio_measurements");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(e => e.ChartId).HasColumnName("chart_id").IsRequired();
        builder.Property(e => e.ToothNumber).HasColumnName("tooth_number").HasMaxLength(5).IsRequired();
        builder.Property(e => e.PdBuccalDb).HasColumnName("pd_buccal_db");
        builder.Property(e => e.PdBuccalB).HasColumnName("pd_buccal_b");
        builder.Property(e => e.PdBuccalMb).HasColumnName("pd_buccal_mb");
        builder.Property(e => e.PdLingualDl).HasColumnName("pd_lingual_dl");
        builder.Property(e => e.PdLingualL).HasColumnName("pd_lingual_l");
        builder.Property(e => e.PdLingualMl).HasColumnName("pd_lingual_ml");
        builder.Property(e => e.RecBuccalDb).HasColumnName("rec_buccal_db").HasDefaultValue((short)0);
        builder.Property(e => e.RecBuccalB).HasColumnName("rec_buccal_b").HasDefaultValue((short)0);
        builder.Property(e => e.RecBuccalMb).HasColumnName("rec_buccal_mb").HasDefaultValue((short)0);
        builder.Property(e => e.RecLingualDl).HasColumnName("rec_lingual_dl").HasDefaultValue((short)0);
        builder.Property(e => e.RecLingualL).HasColumnName("rec_lingual_l").HasDefaultValue((short)0);
        builder.Property(e => e.RecLingualMl).HasColumnName("rec_lingual_ml").HasDefaultValue((short)0);
        builder.Property(e => e.BopBuccalDb).HasColumnName("bop_buccal_db").HasDefaultValue(false);
        builder.Property(e => e.BopBuccalB).HasColumnName("bop_buccal_b").HasDefaultValue(false);
        builder.Property(e => e.BopBuccalMb).HasColumnName("bop_buccal_mb").HasDefaultValue(false);
        builder.Property(e => e.BopLingualDl).HasColumnName("bop_lingual_dl").HasDefaultValue(false);
        builder.Property(e => e.BopLingualL).HasColumnName("bop_lingual_l").HasDefaultValue(false);
        builder.Property(e => e.BopLingualMl).HasColumnName("bop_lingual_ml").HasDefaultValue(false);
        builder.Property(e => e.SupBuccalDb).HasColumnName("sup_buccal_db").HasDefaultValue(false);
        builder.Property(e => e.SupBuccalB).HasColumnName("sup_buccal_b").HasDefaultValue(false);
        builder.Property(e => e.SupBuccalMb).HasColumnName("sup_buccal_mb").HasDefaultValue(false);
        builder.Property(e => e.SupLingualDl).HasColumnName("sup_lingual_dl").HasDefaultValue(false);
        builder.Property(e => e.SupLingualL).HasColumnName("sup_lingual_l").HasDefaultValue(false);
        builder.Property(e => e.SupLingualMl).HasColumnName("sup_lingual_ml").HasDefaultValue(false);
        builder.Property(e => e.FurcationBuccal).HasColumnName("furcation_buccal").HasMaxLength(5);
        builder.Property(e => e.FurcationLingual).HasColumnName("furcation_lingual").HasMaxLength(5);
        builder.Property(e => e.FurcationMesial).HasColumnName("furcation_mesial").HasMaxLength(5);
        builder.Property(e => e.Mobility).HasColumnName("mobility").HasDefaultValue((short)0);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.HasIndex(e => new { e.ChartId, e.ToothNumber }).IsUnique();
    }
}
