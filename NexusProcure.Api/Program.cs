using System.Security.Claims;
using System.Text;
using CloudinaryDotNet;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NexusProcure.Api.Authorization;
using NexusProcure.Api.hangfire;
using NexusProcure.Application;
using NexusProcure.Application.Interfaces;
using NexusProcure.Application.Interfaces.BackgroundJobs;
using NexusProcure.Application.Interfaces.Helper;
using NexusProcure.Application.Interfaces.Procurement;
using NexusProcure.Application.Interfaces.RequestForQuotation;
using NexusProcure.Application.Models;
using NexusProcure.Application.Services;
using NexusProcure.Application.Services.BackgroundJobs;
using NexusProcure.Application.Services.Helper;
using NexusProcure.Application.Services.Procurement;
using NexusProcure.Application.Services.RequestForQuotation;
using NexusProcure.Core.DTOs;
using NexusProcure.Infrastructure.Data;
using Supabase;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new() { Title = "NexusProcure API", Version = "v1" });
//
//     // JWT in Swagger
//     c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//     {
//         Name = "Authorization",
//         Type = SecuritySchemeType.ApiKey,
//         Scheme = "Bearer",
//         BearerFormat = "JWT",
//         In = ParameterLocation.Header,
//         Description = "Enter 'Bearer' [space] and then your valid JWT token."
//     });
//
//     c.AddSecurityRequirement(new OpenApiSecurityRequirement
//     {
//         {
//             new OpenApiSecurityScheme
//             {
//                 Reference = new OpenApiReference
//                 {
//                     Type = ReferenceType.SecurityScheme,
//                     Id = "Bearer"
//                 }
//             },
//             new string[] { }
//         }
//     });
//     // FIX: Handle IFormFile for file uploads in Swagger
//     c.MapType<IFormFile>(() => new OpenApiSchema
//     {
//         Type = "string",
//         Format = "binary"
//     });
//     
// });


// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// DbContext
builder.Services.AddDbContext<NexusProcureDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.Zero,
            NameClaimType = ClaimTypes.Email,
            RoleClaimType = ClaimTypes.Role  
        };
    });

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddSingleton(provider =>
{
    var config = builder.Configuration.GetSection("Cloudinary");
    var account = new Account(
        config["CloudName"],
        config["ApiKey"],
        config["ApiSecret"]
    );
    return new Cloudinary(account);
});

builder.Services.AddSingleton(provider =>
{
    var url = builder.Configuration["Supabase:Url"];
    var key = builder.Configuration["Supabase:ServiceKey"];

    return new Client(url, key);
});

builder.Services.Configure<RiskScoringOptions>(
    builder.Configuration.GetSection("RiskScoring"));

// Application Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<IRequisitionService, RequisitionService>();
builder.Services.AddScoped<IApprovalLevelService, ApprovalLevelService>();
builder.Services.AddScoped<IRequisitionApprovalService, RequisitionApprovalService>();
builder.Services.AddScoped<IApprovalPolicyService, ApprovalPolicyService>();
builder.Services.AddScoped<IRiskScoringService, RiskScoringService>();
builder.Services.AddScoped<IDelegationService, DelegationService>();
builder.Services.AddScoped<ITotalAmountRiskScoreService, TotalAmountRiskScoreService>();
builder.Services.AddScoped<IRequisitionNumberGenerator, RequisitionNumberGenerator>();
builder.Services.AddScoped<IRfqNumberGenerator, RfqNumberGenerator>();
builder.Services.AddScoped<IRfqService, RfqService>();
builder.Services.AddScoped<IRfqExcelService, RfqExcelService>();

// Email
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

// Authorization
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

builder.Services.AddHttpContextAccessor();


// ✅---------------------- HANGFIRE CONFIG ----------------------

// Register background email job service
builder.Services.AddScoped<IEmailJobService, EmailJobService>();
builder.Services.AddScoped<IApprovalEscalationJob, ApprovalEscalationJob>();
builder.Services.AddScoped<IRfqJob, RfqJob>();


builder.Services.AddScoped<HangfireJobLoggingFilter>();

// Register Hangfire with PostgreSQL storage
// builder.Services.AddHangfire(config =>
// {
//     config.UseSimpleAssemblyNameTypeSerializer()
//         .UseRecommendedSerializerSettings()
//         .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
// });
//
//
// // Start Hangfire server
// builder.Services.AddHangfireServer();

// builder.Services.AddHangfireServer(options =>
// {
//     options.Queues = new[] { "default", "rfq" };
// });
//
//
//
// var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionHangFire");
//
// if (!string.IsNullOrEmpty(connectionString))
// {
//     builder.Services.AddHangfire(config =>
//     {
//         config.UseSimpleAssemblyNameTypeSerializer()
//             .UseRecommendedSerializerSettings()
//             .UsePostgreSqlStorage(connectionString);
//     });
//
//     builder.Services.AddHangfireServer();
// }
var hangfireConnection = builder.Configuration
    .GetConnectionString("DefaultConnectionHangFire");

builder.Services.AddHangfire(config =>
{
    config.UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(hangfireConnection);
});

builder.Services.AddHangfireServer(options =>
{
    options.Queues = new[] { "default", "rfq" };
});







// ---------------------------------------------------------------


var app = builder.Build();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new IDashboardAuthorizationFilter[]
    {
        new HangfireBasicAuthFilter("admin", "Admin@123")
    }
});

RecurringJob.AddOrUpdate<IApprovalEscalationJob>(
    "approval-escalation-job",
    job => job.RunAsync(),
    Cron.Hourly
);

RecurringJob.AddOrUpdate<IRfqJob>(
    "rfq-closing-job",
    job => job.ValidateTokenAsync(),
    Cron.Daily
);




// // Swagger
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 403)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\":\"You do not have permission to access this resource.\"}");
    }
});

app.UseAuthorization();
app.MapControllers();

app.Run();
