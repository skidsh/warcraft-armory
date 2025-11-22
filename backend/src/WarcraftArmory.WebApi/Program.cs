using FluentValidation;
using Mapster;
using MapsterMapper;
using Polly;
using Polly.Extensions.Http;
using Refit;
using Scalar.AspNetCore;
using StackExchange.Redis;
using WarcraftArmory.Application.Interfaces;
using WarcraftArmory.Application.Mapping;
using WarcraftArmory.Application.UseCases.Characters.Queries;
using WarcraftArmory.Application.Validation;
using WarcraftArmory.Infrastructure.Caching;
using WarcraftArmory.Infrastructure.ExternalServices.BlizzardApi;
using WarcraftArmory.Infrastructure.ExternalServices.Configuration;
using WarcraftArmory.Infrastructure.Services;
using WarcraftArmory.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ===== Configuration =====
// Load settings from appsettings.json, User Secrets, and environment variables
var blizzardApiSettings = builder.Configuration
    .GetSection("BlizzardApi")
    .Get<BlizzardApiSettings>() ?? new BlizzardApiSettings
    {
        ClientId = "",
        ClientSecret = ""
    };

// Fallback to environment variables with alternate naming (BLIZZARD_CLIENT_ID)
if (string.IsNullOrWhiteSpace(blizzardApiSettings.ClientId))
{
    blizzardApiSettings = blizzardApiSettings with
    {
        ClientId = builder.Configuration["BLIZZARD_CLIENT_ID"] ?? blizzardApiSettings.ClientId,
        ClientSecret = builder.Configuration["BLIZZARD_CLIENT_SECRET"] ?? blizzardApiSettings.ClientSecret
    };
}

// Validate Blizzard API settings
blizzardApiSettings.Validate();

// Register as both singleton instance and IOptions for dependency injection compatibility
builder.Services.AddSingleton(blizzardApiSettings);
builder.Services.AddSingleton<Microsoft.Extensions.Options.IOptions<BlizzardApiSettings>>(
    new Microsoft.Extensions.Options.OptionsWrapper<BlizzardApiSettings>(blizzardApiSettings));

// ===== Logging =====
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ===== Controllers =====
builder.Services.AddControllers();

// ===== OpenAPI =====
builder.Services.AddOpenApi();

// ===== MediatR (CQRS) =====
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(GetCharacterQuery).Assembly);
});

// ===== Mapster (Object Mapping) =====
var mapsterConfig = TypeAdapterConfig.GlobalSettings;
mapsterConfig.Scan(typeof(CharacterMappingConfig).Assembly);
builder.Services.AddSingleton(mapsterConfig);
builder.Services.AddScoped<IMapper, ServiceMapper>();

// ===== FluentValidation =====
builder.Services.AddValidatorsFromAssemblyContaining<GetCharacterRequestValidator>();

// ===== Redis (Distributed Cache) =====
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") 
    ?? builder.Configuration["Redis:ConnectionString"] 
    ?? "localhost:6379";

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(redisConnectionString);
    configuration.AbortOnConnectFail = false;
    configuration.ConnectTimeout = 5000;
    configuration.SyncTimeout = 5000;
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddSingleton<ICacheService, RedisCacheService>();

// ===== Memory Cache =====
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<MemoryCacheService>();

// ===== Distributed Rate Limiter =====
builder.Services.AddSingleton<DistributedRateLimiter>();

// ===== Blizzard API OAuth Authentication =====
builder.Services.AddSingleton<BlizzardAuthService>();

// ===== Polly Resilience Policies =====
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(
        blizzardApiSettings.RetryAttempts,
        retryAttempt => TimeSpan.FromMilliseconds(
            blizzardApiSettings.RetryDelayMilliseconds * Math.Pow(2, retryAttempt - 1)));

var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(
        blizzardApiSettings.CircuitBreakerFailureThreshold,
        TimeSpan.FromSeconds(blizzardApiSettings.CircuitBreakerDurationSeconds));

var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(
    TimeSpan.FromSeconds(blizzardApiSettings.RequestTimeoutSeconds));

// ===== Blizzard OAuth Authentication Service =====
builder.Services.AddHttpClient<BlizzardAuthService>();
builder.Services.AddSingleton<BlizzardAuthService>();

// ===== Authentication Handler =====
builder.Services.AddTransient<BlizzardAuthenticationHandler>();

// ===== Refit HTTP Client for Blizzard API =====
builder.Services.AddRefitClient<IBlizzardApiClient>()
    .ConfigureHttpClient((sp, client) =>
    {
        var region = blizzardApiSettings.DefaultRegion;
        client.BaseAddress = new Uri(blizzardApiSettings.GetApiBaseUrl(region));
        client.Timeout = TimeSpan.FromSeconds(blizzardApiSettings.RequestTimeoutSeconds);
    })
    .AddHttpMessageHandler<BlizzardAuthenticationHandler>()
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy)
    .AddPolicyHandler(timeoutPolicy);

// ===== Blizzard API Service =====
builder.Services.AddScoped<IBlizzardApiService, BlizzardApiService>();

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "http://localhost:80"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// ===== Health Checks =====
builder.Services.AddHealthChecks()
    .AddRedis(redisConnectionString, name: "redis", tags: ["ready"])
    .AddCheck("blizzard-api", () =>
    {
        // Simple health check - can be enhanced to actually ping Blizzard API
        return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Blizzard API configuration loaded");
    }, tags: ["ready"]);

var app = builder.Build();

// ===== Configure HTTP Request Pipeline =====

// Global exception handling (must be first)
app.UseExceptionHandling();

// Development-only middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Warcraft Armory API")
            .WithTheme(ScalarTheme.Purple)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

// CORS
app.UseCors("AllowFrontend");

// HTTPS Redirection
app.UseHttpsRedirection();

// Rate Limiting (per-user)
app.UseRateLimiting();

// Authorization (for future use)
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// Health Checks
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.Run();
