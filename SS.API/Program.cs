using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using SS.Application.Mapping;
using SS.Application.Services;
using SS.Core.Interfaces;
using SS.Infrastructure;
using SS.Infrastructure.Repositories;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secret = jwtSettings.GetValue<string>("Secret");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

#region JWT Authentication
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
        ValidateAudience = true,
        ValidAudience = jwtSettings.GetValue<string>("Audience"),
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(2)
    };
})
#endregion JWT Authentication

#region Google Auth
.AddGoogle("Google", options =>
{
    options.ClientId = builder.Configuration["Google:ClientId"];
    options.ClientSecret = builder.Configuration["Google:ClientSecret"];
    options.SaveTokens = true;
    // For APIs you might set a CallbackPath or Events to issue JWT on successful sign-in
    options.Events.OnCreatingTicket = ctx =>
    {
        // You can read ctx.Principal, create/find user, then generate your JWT and redirect
        return Task.CompletedTask;
    };
}
#endregion Google Auth
);
#endregion Authentication

#region Authorization
builder.Services.AddAuthorization(options =>
{
    // you can add policies if you want more control
    options.AddPolicy("RequireSuperAdmin", policy => policy.RequireRole("SuperAdmin"));
});
#endregion Authorization

#region Log4Net Config
var logsPath = Path.Combine(AppContext.BaseDirectory, "Logs");
if (!Directory.Exists(logsPath))
{
    Directory.CreateDirectory(logsPath);
}

var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

builder.Logging.ClearProviders();
builder.Logging.AddLog4Net("log4net.config");
#endregion Log4Net Config


builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon")));

builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<JobServices>();
builder.Services.AddScoped<IApplyJobRepository, ApplyJobRepository>();
builder.Services.AddScoped<ApplyJobServices>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<UserProfileService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

#region Input JWT Token
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Your API",
        Version = "v1"
    });

    // Add JWT Authentication
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token like: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

#endregion Input JWT Token

#region AddCors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAngularClient", policy =>
//    {
//        policy
//            .AllowAnyOrigin()   // OR .WithOrigins("http://localhost:4200")
//            .AllowAnyHeader()
//            .AllowAnyMethod()
//            .WithExposedHeaders("Authorization");
//    });
//});
#endregion AddCors

var app = builder.Build();

#region Super Admin Auto Created
using (var scope = app.Services.CreateScope())
{
    var authService = scope.ServiceProvider.GetRequiredService<AuthService>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var superEmail = config["SuperAdmin:Email"];
    var superPassword = config["SuperAdmin:Password"];
    if (!string.IsNullOrEmpty(superEmail) && !string.IsNullOrEmpty(superPassword))
    {
        await authService.EnsureSuperAdminExists(superEmail, superPassword);
    }
}

#endregion Super Admin Auto Created

#region LoggerApi {/}
app.MapGet("/", (ILogger<Program> logger) =>
{
    logger.LogDebug("Debug message");
    logger.LogInformation("Info message");
    logger.LogWarning("Warning message");
    logger.LogError("Error message");
    logger.LogCritical("Fatal message");

    return "Log4Net Partially Working!";
});
#endregion LoggerApi {/}

#region File Uploads

var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "Uploads", "Resumes");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

// ENABLE static files
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "Uploads")
    ),
    RequestPath = "/Uploads"
});

#endregion File Uploads
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowAngularClient");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
