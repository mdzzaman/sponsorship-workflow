using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SponsorshipWorkflow.API.Authorization;
using SponsorshipWorkflow.API.Middleware;
using SponsorshipWorkflow.Application;
using SponsorshipWorkflow.Domain.Enums;
using SponsorshipWorkflow.Infrastructure;
using SponsorshipWorkflow.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Render.com provides a postgres:// URL; Npgsql requires key=value format
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
if (connStr != null && (connStr.StartsWith("postgres://") || connStr.StartsWith("postgresql://")))
{
    var uri = new Uri(connStr);
    var userInfo = uri.UserInfo.Split(':');
    var port = uri.Port == -1 ? 5432 : uri.Port;
    builder.Configuration["ConnectionStrings:DefaultConnection"] =
        $"Host={uri.Host};Port={port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={Uri.UnescapeDataString(userInfo[1])}";
}

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sponsorship Workflow API",
        Version = "v1",
        Description = "Sponsorship Request Approval Workflow — Senior .NET Assessment"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token (without 'Bearer ' prefix)"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            jwtSettings["Secret"] ?? throw new InvalidOperationException("JwtSettings:Secret is not configured.")))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.IsRequestor,  p => p.RequireRole(UserRole.Requestor));
    options.AddPolicy(Policies.CanApprove,   p => p.RequireRole(UserRole.Manager, UserRole.FinanceAdmin));
    options.AddPolicy(Policies.IsSystemAdmin, p => p.RequireRole(UserRole.SystemAdmin));
});

// CORS
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

var app = builder.Build();

// Auto-migrate and seed on startup
using (var scope = app.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
    await initialiser.InitialiseAsync();
    await initialiser.SeedAsync();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sponsorship Workflow API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
