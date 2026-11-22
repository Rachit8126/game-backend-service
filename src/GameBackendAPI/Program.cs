using GameBackendAPI.Data;
using GameBackendAPI.Security;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var sqlUsername = GetConfigurationKey("POSTGRES:USERNAME", builder);
var sqlPassword = GetConfigurationKey("POSTGRES:PASSWORD", builder);
var sqlDatabase = GetConfigurationKey("POSTGRES:DATABASE", builder);
var host = GetConfigurationKey("POSTGRES:HOST", builder);
var port = GetConfigurationKey("POSTGRES:PORT", builder);

if (!int.TryParse(port, out int parsedPort))
{
    throw new InvalidOperationException("POSTGRES:PORT must be a number");
}

var connectionStringBuilder = new NpgsqlConnectionStringBuilder
{
    Host = host,
    Port = parsedPort,
    Username = sqlUsername,
    Password = sqlPassword,
    Database = sqlDatabase
};

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSingleton<ICustomPasswordHasher, SodiumHasher>();

builder.Services.AddHealthChecks().AddDbContextCheck<PlayerContext>(tags: ["readiness"]).AddCheck("self", () => HealthCheckResult.Healthy(), ["liveness"]);
builder.Services.AddNpgsql<PlayerContext>(connectionStringBuilder.ConnectionString);
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapHealthChecks("/health-readiness", new() { Predicate = c => c.Tags.Contains("readiness") });
app.MapHealthChecks("/health-liveness", new() { Predicate = c => c.Tags.Contains("liveness") });

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

static string GetConfigurationKey(string key, WebApplicationBuilder builder)
{
    var value = builder.Configuration[key] ?? throw new InvalidOperationException($"{key} key is missing");

    return value;
}

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
