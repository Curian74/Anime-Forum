﻿using Microsoft.AspNetCore.Identity;
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
using System.Text.Json.Serialization;
using Infrastructure.External;
using Application.Interfaces.Email;
using Application.Common.EmailTemplate;
using Application.Common.File;
using Application.Common.MessageOperations;
using Application.Hubs;
using Microsoft.AspNetCore.SignalR;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; //Ignore cycles
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; //Ignore null objects
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Dependency injections

// Persistence classes
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Service classes
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<TemplateBody>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<PostCategoryService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<VoteService>();
builder.Services.AddScoped<MediaService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<RankService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<Application.Services.NotificationService>();

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
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddSignalR();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();


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
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue(builder.Configuration["AuthTokenOptions:Name"], out var token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    });

// Add the authorization middleware with hierarchy-based auth policies
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("MemberPolicy", policy => policy.RequireRole(UserRoles.Member, UserRoles.Moderator, UserRoles.Admin))
    .AddPolicy("ModeratorPolicy", policy => policy.RequireRole(UserRoles.Moderator, UserRoles.Admin))
    .AddPolicy("AdminPolicy", policy => policy.RequireRole(UserRoles.Admin));

// Auth token configuration
builder.Services.Configure<AuthTokenOptions>(
    builder.Configuration.GetSection("AuthTokenOptions")
);


// Cookie configuration + CORS
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder
            .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost") // Allow any localhost port
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod());
});


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

    // Seed 3 accounts, for testing purposes. Username and password are the same and is the name of their roles
    var userManager = services.GetRequiredService<UserManager<User>>();

    var passwordValidators = userManager.PasswordValidators;
    var userValidators = userManager.UserValidators;
    userManager.PasswordValidators.Clear();
    userManager.UserValidators.Clear();

    var admin = await userManager.FindByNameAsync("admin");
    if (admin == null)
    {
        admin = new User
        {
            Id = Guid.NewGuid(),
            UserName = "admin",
            Email = "admin@animeforum.com",
            Bio = "admin"
        };

        await userManager.CreateAsync(admin, "admin");
        await userManager.AddToRoleAsync(admin, UserRoles.Admin);
    }

    var moderator = await userManager.FindByNameAsync("moderator");
    if (moderator == null)
    {
        moderator = new User
        {
            Id = Guid.NewGuid(),
            UserName = "moderator",
            Email = "moderator@animeforum.com",
            Bio = "moderator"
        };

        await userManager.CreateAsync(moderator, "moderator");
        await userManager.AddToRoleAsync(moderator, UserRoles.Moderator);
    }

    var member = await userManager.FindByNameAsync("member");
    if (member == null)
    {
        member = new User
        {
            Id = Guid.NewGuid(),
            UserName = "member",
            Email = "member@animeforum.com",
            Bio = "member"
        };

        await userManager.CreateAsync(member, "member");
        await userManager.AddToRoleAsync(member, UserRoles.Member);
    }

    foreach (var validator in passwordValidators)
    {
        userManager.PasswordValidators.Add(validator);
    }

    foreach (var validator in userValidators)
    {
        userManager.UserValidators.Add(validator);
    }

    // Seed category table with categories
    if (!db.PostCategories.Any())
    {
        var categories = new List<PostCategory>
        {
            new() { Id = Guid.NewGuid(), Name = "Thông báo", IsRestricted = true},
            new() { Id = Guid.NewGuid(), Name = "Tin tức", IsRestricted = true},
            new() { Id = Guid.NewGuid(), Name = "Chia sẻ kiến thức"},
            new() { Id = Guid.NewGuid(), Name = "Review linh tinh"},
            new() { Id = Guid.NewGuid(), Name = "Review manga"},
            new() { Id = Guid.NewGuid(), Name = "Review anime"},
            new() { Id = Guid.NewGuid(), Name = "Văn hóa Nhật"},
            new() { Id = Guid.NewGuid(), Name = "Hỏi đáp"},
        };

        db.PostCategories.AddRange(categories);
    }

    // Seed rank table with ranks
    if (!db.Ranks.Any())
    {
        var ranks = new List<Rank>
        {
            new() { Id = Guid.NewGuid(), Name = "Lính mới", PointsRequired = 0 },
            new() { Id = Guid.NewGuid(), Name = "Công dân hạng C", PointsRequired = 50 },
            new() { Id = Guid.NewGuid(), Name = "Wibu", PointsRequired = 150 },
            new() { Id = Guid.NewGuid(), Name = "Wibu kỳ cựu", PointsRequired = 450 },
            new() { Id = Guid.NewGuid(), Name = "Tộc trưởng wibu", PointsRequired = 1350 },
            new() { Id = Guid.NewGuid(), Name = "Nhật nội địa", PointsRequired = 4050 },
            new() { Id = Guid.NewGuid(), Name = "Wibu chúa", PointsRequired = 12150 },
        };

        db.Ranks.AddRange(ranks);
    }

    // Seed user flair table with user flairs
    if (!db.UserFlairs.Any())
    {
        var flairs = new List<UserFlair>
        {
            new() { Id = Guid.NewGuid(), Name = "Lính mới", ColorHex = "#FF0000", PointsRequired = 0 }, // red
            new() { Id = Guid.NewGuid(), Name = "Công dân hạng C", ColorHex = "#FF7F00", PointsRequired = 2 }, // orange
            new() { Id = Guid.NewGuid(), Name = "Wibu", ColorHex = "#FFFF00", PointsRequired = 3 }, // yellow
            new() { Id = Guid.NewGuid(), Name = "Wibu kỳ cựu", ColorHex = "#00FF00", PointsRequired = 4 }, // green
            new() { Id = Guid.NewGuid(), Name = "Tộc trưởng wibu", ColorHex = "#FF0000", PointsRequired = 5 }, // blue
            new() { Id = Guid.NewGuid(), Name = "Nhật nội địa", ColorHex = "#4B0082", PointsRequired = 6 }, // indigo
            new() { Id = Guid.NewGuid(), Name = "Wibu chúa", ColorHex = "#8B00FF", PointsRequired = 7 }, // violet
        };

        db.UserFlairs.AddRange(flairs);
    }


    if (db.ChangeTracker.HasChanges())
    {
        await db.SaveChangesAsync();
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapHub<NotificationHub>("/notificationHub");

app.UseCors("AllowFrontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
