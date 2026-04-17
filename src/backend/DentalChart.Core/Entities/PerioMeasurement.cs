using System.Text.Json.Serialization;

namespace DentalChart.Core.Entities;

public class PerioMeasurement
{
    public Guid Id { get; set; }
    public Guid ChartId { get; set; }
    public string ToothNumber { get; set; } = string.Empty;
    public short? PdBuccalDb { get; set; }
    public short? PdBuccalB { get; set; }
    public short? PdBuccalMb { get; set; }
    public short? PdLingualDl { get; set; }
    public short? PdLingualL { get; set; }
    public short? PdLingualMl { get; set; }
    public short RecBuccalDb { get; set; }
    public short RecBuccalB { get; set; }
    public short RecBuccalMb { get; set; }
    public short RecLingualDl { get; set; }
    public short RecLingualL { get; set; }
    public short RecLingualMl { get; set; }
    public bool BopBuccalDb { get; set; }
    public bool BopBuccalB { get; set; }
    public bool BopBuccalMb { get; set; }
    public bool BopLingualDl { get; set; }
    public bool BopLingualL { get; set; }
    public bool BopLingualMl { get; set; }
    public bool SupBuccalDb { get; set; }
    public bool SupBuccalB { get; set; }
    public bool SupBuccalMb { get; set; }
    public bool SupLingualDl { get; set; }
    public bool SupLingualL { get; set; }
    public bool SupLingualMl { get; set; }
    public string? FurcationBuccal { get; set; }
    public string? FurcationLingual { get; set; }
    public string? FurcationMesial { get; set; }
    public short Mobility { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonIgnore]
    public PerioChart? Chart { get; set; }
}
