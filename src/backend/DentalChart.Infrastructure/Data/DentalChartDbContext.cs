using Microsoft.EntityFrameworkCore;

namespace DentalChart.Infrastructure.Data;

public sealed class DentalChartDbContext(DbContextOptions<DentalChartDbContext> options) : DbContext(options)
{
}
