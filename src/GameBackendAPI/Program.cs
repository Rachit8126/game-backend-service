using GameBackendAPI.Data;
using GameBackendAPI.Endpoints;
using GameBackendAPI.HealthChecks;
using GameBackendAPI.OpenAPI;
using GameBackendAPI.Security.Authentication;
using GameBackendAPI.Security.Hashing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Scalar.AspNetCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var psqlUsername = GetConfigurationKey("POSTGRES:USERNAME", builder);
var psqlPassword = GetConfigurationKey("POSTGRES:PASSWORD", builder);
var psqlDatabase = GetConfigurationKey("POSTGRES:DATABASE", builder);

var psqlHost = GetConfigurationKey("POSTGRES:HOST", builder);
var psqlPort = GetConfigurationKey("POSTGRES:PORT", builder);

var redisHost = GetConfigurationKey("REDIS:HOST", builder);
var redisPort = GetConfigurationKey("REDIS:PORT", builder);

if (!int.TryParse(psqlPort, out int parsedPsqlPort))
{
    throw new InvalidOperationException("POSTGRES:PORT must be a number");
}

if (!int.TryParse(redisPort, out int parsedRedisPort))
{
    throw new InvalidOperationException("REDIS:PORT must be a number");
}

var connectionStringBuilder = new NpgsqlConnectionStringBuilder
{
    Host = psqlHost,
    Port = parsedPsqlPort,
    Username = psqlUsername,
    Password = psqlPassword,
    Database = psqlDatabase
};

var configurationOptions = new ConfigurationOptions();
configurationOptions.EndPoints.Add($"{redisHost}:{redisPort}");
configurationOptions.AbortOnConnectFail = false;

builder.Services.AddSingleton<IConnectionMultiplexer>(p => ConnectionMultiplexer.Connect(configurationOptions));
builder.Services.AddSingleton<ICustomPasswordHasher, SodiumHasher>();
builder.Services.AddSingleton<IAuthTokenService, AuthTokenService>();

builder.Services.AddValidation();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddAuthorization();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<DatabaseContext>(tags: ["readiness"]).AddCheck("self", () => HealthCheckResult.Healthy(), ["liveness"])
    .AddCheck<RedisHealthCheck>("redisCheck", tags: ["readiness"]);

builder.Services.AddNpgsql<DatabaseContext>(connectionStringBuilder.ConnectionString);
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

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi(o =>
{
    o.AddDocumentTransformer<OpenApiDocumentTransformer>();
    o.AddOperationTransformer<OpenApiOperationTransformer>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
else
{
    app.UseExceptionHandler();
}

app.UseStatusCodePages();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health-readiness", new() { Predicate = c => c.Tags.Contains("readiness") });
app.MapHealthChecks("/health-liveness", new() { Predicate = c => c.Tags.Contains("liveness") });

app.MapPlayerEndpoints();
app.MapAuthEndpoints();
app.MapScoreEndpoints();

app.Run();

static string GetConfigurationKey(string key, WebApplicationBuilder builder)
{
    var value = builder.Configuration[key] ?? throw new InvalidOperationException($"{key} key is missing");

    return value;
}
