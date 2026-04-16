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
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "QuantityMeasurementApp",
            ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? "QuantityMeasurementApp",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"] ?? "super_secret_key_that_is_long_enough_for_hmac_sha256"))
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
        // Handle Postgres URL conversion if needed, though Npgsql 10.x handles postgres:// directly.
        // If it's the postgres:// format, we can just pass it directly or do the manual conversion.
        var finalConnString = connectionString;
        if (connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
        {
            try 
            {
                var databaseUri = new Uri(connectionString);
                var userInfo = databaseUri.UserInfo.Split(':', 2);
                var pgHost = databaseUri.Host;
                var pgPort = databaseUri.Port;
                var pgDatabase = databaseUri.AbsolutePath.TrimStart('/');
                
                if (userInfo.Length == 2)
                {
                    var pgUser = userInfo[0];
                    var pgPassword = userInfo[1];
                    finalConnString = $"Host={pgHost};Port={pgPort};Database={pgDatabase};Username={pgUser};Password={pgPassword};Pooling=true;Trust Server Certificate=true";
                }
                else
                {
                    // Fallback to the original URL and let Npgsql handle it
                    finalConnString = connectionString;
                }
            }
            catch 
            {
                // If URI parsing fails, use original
                finalConnString = connectionString;
            }
        }
        
        options.UseNpgsql(finalConnString);
    }
    else
    {
        // Assume SQLite
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
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();