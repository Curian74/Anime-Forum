using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Domain.Entities;
using Application.Common.Mappings;
using Application.Services;
using Domain.Common.Roles;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Infrastructure.Extensions;
using Infrastructure.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Dependency injections

// Persistence classes
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddScoped<DbContext, ApplicationDbContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Configuration manager
builder.Services.AddScoped<Domain.Interfaces.IConfigurationManager, Infrastructure.Configurations.ConfigurationManager>();

// Service classes
builder.Services.AddScoped<PostServices>();
builder.Services.AddScoped<UserServices>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<TicketServices>();

// AutoMapper service
// Quet project, tim tat ca file MappingProfile roi gop lai thanh 1
// Mapping profile co san trong /Application/Common/Mappings/
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Helper services
builder.Services.AddScoped<JwtHelper>();
//builder.Services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

#endregion

// EF Identity configurations
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<RoleManager<IdentityRole<Guid>>>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
        };
    });

// Auth token configuration
builder.Services.Configure<AuthTokenOptions>(
    builder.Configuration.GetSection("AuthTokenOptions")
);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Update database to use the lastest migrations, ignored if database is up to date
    var db = services.GetRequiredService<ApplicationDbContext>();

    var pendingMigrations = db.Database.GetPendingMigrations().ToList();
    if (pendingMigrations.Count != 0)
    {
        db.Database.Migrate();
    }

    // Add the roles
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
    {
        await roleManager.CreateAsync(new IdentityRole<Guid>(UserRoles.Admin));
    }

    if (!await roleManager.RoleExistsAsync(UserRoles.Moderator))
    {
        await roleManager.CreateAsync(new IdentityRole<Guid>(UserRoles.Moderator));
    }

    if (!await roleManager.RoleExistsAsync(UserRoles.Member))
    {
        await roleManager.CreateAsync(new IdentityRole<Guid>(UserRoles.Member));
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseAuthentication();

app.MapControllers();

app.Run();
