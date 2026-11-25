using GameBackendAPI.Data;
using GameBackendAPI.Endpoints;
using GameBackendAPI.Security.Authentication;
using GameBackendAPI.Security.Hashing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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

builder.Services.AddSingleton<ICustomPasswordHasher, SodiumHasher>();
builder.Services.AddSingleton<IAuthTokenService, AuthTokenService>();

builder.Services.AddValidation();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddAuthorization();

builder.Services.AddHealthChecks().AddDbContextCheck<PlayerContext>(tags: ["readiness"]).AddCheck("self", () => HealthCheckResult.Healthy(), ["liveness"]);

builder.Services.AddNpgsql<PlayerContext>(connectionStringBuilder.ConnectionString);
builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme).Configure<IOptions<AuthSettings>>((jwt, auth) =>
{
    jwt.MapInboundClaims = false;
    jwt.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidIssuer = auth.Value.Issuer,
        ValidateAudience = true,
        ValidAudience = auth.Value.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = auth.Value.GetSymmetricSecurityKey(),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(10),
        ValidAlgorithms = [SecurityAlgorithms.HmacSha256]
    };
});
builder.Services.AddOptions<AuthSettings>().Bind(builder.Configuration.GetSection("Auth")).ValidateDataAnnotations().ValidateOnStart();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health-readiness", new() { Predicate = c => c.Tags.Contains("readiness") });
app.MapHealthChecks("/health-liveness", new() { Predicate = c => c.Tags.Contains("liveness") });

app.MapPlayerEndpoints();
app.MapAuthEndpoints();

app.Run();

static string GetConfigurationKey(string key, WebApplicationBuilder builder)
{
    var value = builder.Configuration[key] ?? throw new InvalidOperationException($"{key} key is missing");

    return value;
}
