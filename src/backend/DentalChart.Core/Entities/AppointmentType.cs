namespace DentalChart.Core.Entities;

public class AppointmentType
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Abbreviation { get; set; }
    public int DefaultDurationMinutes { get; set; } = 60;
    public string Color { get; set; } = "#3182CE";
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
