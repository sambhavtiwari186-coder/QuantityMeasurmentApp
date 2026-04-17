using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.RateLimiting;
using QuantityMeasurementApp.Repository;
using QuantityMeasurementApp.Repository.Database;
using QuantityMeasurementApp.Repository.Sync;
using QuantityMeasurementApp.Service;
using QuantityMeasurementApp.Service.Interface;
using QuantityMeasurementWebAPI.Middleware;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Swagger/OpenAPI with JWT Authorization button
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Quantity Measurement API",
        Version = "v1",
        Description = "API for converting, comparing, and performing arithmetic on quantities"
    });
    
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Security: Add JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretKey = builder.Configuration["JwtSettings:SecretKey"] ?? "ThisIsAVeryLongSecretKeyThatIsDefinitelyLongEnoughForHMACSHA256AlgorithmAndShouldWorkProperlyInProductionEnvironment";
        var issuer = builder.Configuration["JwtSettings:Issuer"] ?? "QuantityMeasurementApp";
        var audience = builder.Configuration["JwtSettings:Audience"] ?? "QuantityMeasurementApp";

        Console.WriteLine($"[JWT] SecretKey length: {secretKey.Length}");
        Console.WriteLine($"[JWT] Issuer: {issuer}");
        Console.WriteLine($"[JWT] Audience: {audience}");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

// Security: OAuth-2 placeholder
// builder.Services.AddAuthentication().AddGoogle(options => { ... });

// REST API Security: Add CORS Support
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// REST API Security: Add Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("FixedPolicy", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 100;
        opt.QueueLimit = 0;
    });
});


// Configure Entity Framework Core with SQLite/Postgres based on environment.
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=quantity_measurement.db";
var databaseUrl = builder.Configuration["DATABASE_URL"];

// Preference order: 
// 1. DATABASE_URL (usually Postgres on Render)
// 2. DefaultConnection from appsettings.json
// 3. Hardcoded fallback
var connectionString = !string.IsNullOrWhiteSpace(databaseUrl) ? databaseUrl : defaultConnection;

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (!string.IsNullOrWhiteSpace(connectionString) && 
        (connectionString.StartsWith("postgres", StringComparison.OrdinalIgnoreCase) || 
         connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase)))
    {
        var finalConnString = connectionString;
        
        // If it's a URI format (postgres://), convert it to standard Host=... format for better compatibility
        if (connectionString.StartsWith("postgres", StringComparison.OrdinalIgnoreCase))
        {
            try 
            {
                // Fix for postgres vs postgresql scheme
                var uriString = connectionString.StartsWith("postgres://") ? 
                    connectionString.Replace("postgres://", "postgresql://") : connectionString;
                
                var databaseUri = new Uri(uriString);
                var userInfo = databaseUri.UserInfo.Split(':', 2);
                var pgHost = databaseUri.Host;
                var pgPort = databaseUri.Port > 0 ? databaseUri.Port : 5432;
                var pgDatabase = databaseUri.AbsolutePath.TrimStart('/');
                
                if (userInfo.Length == 2)
                {
                    var pgUser = userInfo[0];
                    var pgPassword = userInfo[1];
                    finalConnString = $"Host={pgHost};Port={pgPort};Database={pgDatabase};Username={pgUser};Password={pgPassword};Pooling=true;SSL Mode=Require;Trust Server Certificate=true;";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB] URI parsing failed, falling back to original: {ex.Message}");
                // Ensure no spaces if appending to URL
                if (!finalConnString.Contains("TrustServerCertificate=", StringComparison.OrdinalIgnoreCase))
                {
                    finalConnString += finalConnString.Contains("?") ? "&" : "?";
                    finalConnString += "TrustServerCertificate=true";
                }
            }
        }
        else if (!finalConnString.Contains("Trust Server Certificate=", StringComparison.OrdinalIgnoreCase))
        {
            finalConnString = finalConnString.TrimEnd(';') + ";Trust Server Certificate=true;";
        }
        
        // Log the connection attempt (masking password)
        var logString = finalConnString;
        if (logString.Contains("Password="))
        {
            var pStart = logString.IndexOf("Password=", StringComparison.OrdinalIgnoreCase);
            var pEnd = logString.IndexOf(";", pStart);
            if (pEnd == -1) pEnd = logString.Length;
            logString = logString.Remove(pStart + 9, pEnd - (pStart + 9)).Insert(pStart + 9, "****");
        }
        Console.WriteLine($"[DB] Using PostgreSQL connection: {logString}");
        
        options.UseNpgsql(finalConnString);
    }
    else
    {
        Console.WriteLine($"[DB] Using SQLite connection: {connectionString}");
        options.UseSqlite(connectionString);
    }
});

// Configure Redis Cache (Disabled as per configuration change)
// builder.Services.AddStackExchangeRedisCache(...) is disabled.


builder.Services.AddDistributedMemoryCache();


// Configure Repositories
builder.Services.AddScoped<QuantityMeasurementDatabaseRepository>();
builder.Services.AddScoped<IQuantityMeasurementRepository>(provider => 
    new QuantityMeasurementSyncRepository(provider.GetRequiredService<QuantityMeasurementDatabaseRepository>())
);

// Configure Services
builder.Services.AddScoped<IQuantityMeasurementService, QuantityMeasurementService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Configure Background Service for Sync
builder.Services.AddHostedService<PendingSyncBackgroundService>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandler>();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Quantity Measurement API v1");
});

if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // REST API Security: HSTS enabled in prod
}

// app.UseHttpsRedirection();

// REST API Security middlewares
app.UseCors("AllowAll");
app.UseRateLimiter();

app.UseDefaultFiles();
app.UseStaticFiles();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    try 
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Console.WriteLine("[DB] Checking database state...");
        db.Database.EnsureCreated();
        Console.WriteLine("[DB] Database is ready.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[DB] Error during database initialization: {ex.Message}");
        // We don't rethrow here to allow the app to actually start and show errors in the logs/Swagger
    }
}

app.Run();