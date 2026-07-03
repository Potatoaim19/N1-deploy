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
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                      ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions => {
        sqlOptions.EnableRetryOnFailure();
    }));

// 6. Đăng ký HttpClient
builder.Services.AddHttpClient("AuthService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ExternalServices:PaymentAuthBaseUrl"] ?? "http://localhost:5203");
});

// 7. Đăng ký Auth
var rawJwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? builder.Configuration["Jwt:Key"]
                ?? "sbEl82-7Rcec7ezEQgAHYJb-uXX7SaLAXgLCoZtIQIep5hKibwdWkIzKkbD-KumM";

var jwtSecret = rawJwtSecret.Trim().Trim('"');
var jwtIssuer = (Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "TrainingCenter.Auth").Trim().Trim('"');
var jwtAudience = (Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "TrainingCenter.Api").Trim().Trim('"');

// Danh sách các Key tiềm năng để giải mã
var signingKeys = new List<SecurityKey> {
    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
};
try {
    // Thử thêm bản Base64 (phòng trường hợp Service 3 dùng Base64)
    signingKeys.Add(new SymmetricSecurityKey(Convert.FromBase64String(jwtSecret)));
} catch { }

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
        IssuerSigningKeys = signingKeys, // Thử tất cả các key
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.NameIdentifier,
        ClockSkew = TimeSpan.FromMinutes(5)
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"JWT Auth Failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("JWT Auth SUCCESS!");
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// CẤU HÌNH PIPELINE (Đúng thứ tự Leader yêu cầu)
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Course Schedule API V1");
    c.RoutePrefix = "swagger";
});

app.UseRouting(); // Thêm explicit Routing
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapGet("/", () => "Course Schedule API is running!");
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
