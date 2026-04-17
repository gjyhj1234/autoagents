using DentalChart.Api.Contracts;
using DentalChart.Api.Endpoints;
using DentalChart.Api.Serialization;
using DentalChart.Infrastructure;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, DentalChartJsonContext.Default);
});

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var origins = GetAllowedOrigins(builder.Configuration);
        if (origins.Length == 0)
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            return;
        }

        policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => TypedResults.Redirect("/health"));

app.MapGet("/health", () => TypedResults.Ok(new HealthResponse("healthy", DateTimeOffset.UtcNow)))
    .WithName("HealthCheck");

app.MapAuthEndpoints();
app.MapPatientEndpoints();

app.Run();

static string[] GetAllowedOrigins(ConfigurationManager configuration)
{
    var configuredOrigins = configuration["CORS_ALLOWED_ORIGINS"];
    return string.IsNullOrWhiteSpace(configuredOrigins)
        ? []
        : configuredOrigins.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
}

public partial class Program;
