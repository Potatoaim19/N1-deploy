using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Render cấp PORT, local dùng 8080
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Load ocelot.json
builder.Configuration.AddJsonFile(
    "ocelot.json",
    optional: false,
    reloadOnChange: true
);

// Đọc JWT theo chuẩn thống nhất của dự án
var jwtSecret =
    Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? builder.Configuration["Jwt:Key"];

var jwtIssuer =
    Environment.GetEnvironmentVariable("JWT_ISSUER")
    ?? builder.Configuration["Jwt:Issuer"];

var jwtAudience =
    Environment.GetEnvironmentVariable("JWT_AUDIENCE")
    ?? builder.Configuration["Jwt:Audience"];

// ... (phần code CORS giữ nguyên) ...

// Cấu hình JWT cho Gateway.
if (!string.IsNullOrWhiteSpace(jwtSecret))
{
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer("Bearer", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,

                ValidateAudience = true,
                ValidAudience = jwtAudience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSecret)
                ),

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2),

                // Thống nhất Claim Types
                NameClaimType = ClaimTypes.NameIdentifier,
                RoleClaimType = ClaimTypes.Role
            };
        });

    builder.Services.AddAuthorization();
}

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.UseCors("FrontendCors");

app.MapGet("/", () => Results.Ok(new
{
    app = "Training Center API Gateway",
    status = "Running"
}));

app.MapGet("/health", () => Results.Ok(new
{
    app = "Training Center API Gateway",
    status = "Healthy",
    time = DateTime.UtcNow
}));

app.UseAuthentication();
app.UseAuthorization();

// Chỉ cho Ocelot xử lý các request bắt đầu bằng /api
// để /health không bị Ocelot bắt nhầm.
app.MapWhen(
    context => context.Request.Path.StartsWithSegments("/api"),
    apiApp =>
    {
        apiApp.UseOcelot().Wait();
    }
);

app.Run();