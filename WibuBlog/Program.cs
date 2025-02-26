using Domain.Common.Roles;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WibuBlog.Interfaces.Api;
using WibuBlog.Services;
using WibuBlog.Services.Api;

namespace WibuBlog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //DbContext
            builder.Services.AddDbContext<ApplicationDbContext>();
            builder.Services.AddScoped<DbContext, ApplicationDbContext>();

            //Dependency injections
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<Domain.Interfaces.IConfigurationManager, Infrastructure.Configurations.ConfigurationManager>();
            builder.Services.AddScoped<IApiServices, ApiServices>();
            builder.Services.AddScoped<PostServices>();
            builder.Services.AddScoped<PostCategoryServices>();
            builder.Services.AddScoped<TicketServices>();

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
    })
    .AddCookie(options => {
        options.LoginPath = "/Authentication/Login"; // Where to redirect when not logged in
        options.AccessDeniedPath = "/Authentication/AccessDenied"; // For unauthorized access
    });

            builder.Services.AddAuthorizationBuilder()
    .AddPolicy("MemberPolicy", policy => policy.RequireRole(UserRoles.Member, UserRoles.Moderator, UserRoles.Admin))
    .AddPolicy("ModeratorPolicy", policy => policy.RequireRole(UserRoles.Moderator, UserRoles.Admin))
    .AddPolicy("AdminPolicy", policy => policy.RequireRole(UserRoles.Admin));

            //HttpClient
            builder.Services.AddHttpClient("api", httpClient =>
            {
                httpClient.BaseAddress = new Uri("http://localhost:5002/api/");
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseStatusCodePages(context =>
            {
                var response = context.HttpContext.Response;

                if (response.StatusCode == 401)
                {
                    response.Redirect("/Authentication/Login");
                }
                else if (response.StatusCode == 403)
                {
                    response.Redirect("/Authentication/AccessDenied");
                }

                return Task.CompletedTask;
            });

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
