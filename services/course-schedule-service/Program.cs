using System.Text;
using System.Text.Json.Serialization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using N1.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký Controllers và JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// 2. Đăng ký CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// 3. Đăng ký Swagger (Bắt buộc phải có đoạn này)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Course Schedule API", Version = "v1" });
});

// 4. Đăng ký Health Checks
builder.Services.AddHealthChecks();

// 5. Đăng ký Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 6. Đăng ký HttpClient
builder.Services.AddHttpClient("AuthService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ExternalServices:PaymentAuthBaseUrl"] ?? "http://localhost:5203");
});

// 7. Đăng ký Auth
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? builder.Configuration["Jwt:Key"]
                ?? "sbEl82-7Rcec7ezEQgAHYJb-uXX7SaLAXgLCoZtIQIep5hKibwdWkIzKkbD-KumM";

var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
                ?? builder.Configuration["Jwt:Issuer"]
                ?? "TrainingCenter.Auth";

var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                  ?? builder.Configuration["Jwt:Audience"]
                  ?? "TrainingCenter.Api";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,

        ValidateAudience = true,
        ValidAudience = jwtAudience,

        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),

        // Đảm bảo đọc đúng Role từ Service 3
        RoleClaimType = ClaimTypes.Role, // http://schemas.microsoft.com/ws/2008/06/identity/claims/role
        NameClaimType = ClaimTypes.NameIdentifier // http://schemas.microsoft.com/ws/2008/06/identity/claims/nameidentifier
    };

    // Thêm log để debug nếu cần (optional)
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("JWT Auth Failed: " + context.Exception.Message);
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// CẤU HÌNH PIPELINE
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Course Schedule API V1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowAll");
app.MapHealthChecks("/health");
app.MapGet("/", () => "Course Schedule API is running!");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Khởi tạo Database âm thầm
try {
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        DbSeeder.Seed(context);
    }
} catch (Exception ex) {
    Console.WriteLine("DB Seed Error: " + ex.Message);
}

app.Run();
