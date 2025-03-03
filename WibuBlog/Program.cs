using Application.Common.EmailTemplate;
using Application.Interfaces.Email;
using Domain.Common.Roles;
using Domain.Interfaces;
using Infrastructure.Configurations;
using Infrastructure.External;
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
            builder.Services.AddScoped<IApiService, ApiService>();
            builder.Services.AddScoped<PostService>();
            builder.Services.AddScoped<PostCategoryService>();
            builder.Services.AddScoped<TicketService>();
            builder.Services.AddScoped<UserService>();
			builder.Services.AddScoped<TemplateBody>();
			builder.Services.AddScoped<CommentService>();
			builder.Services.AddScoped<AuthService>();
			builder.Services.AddScoped<OtpService>();
            builder.Services.AddScoped<IEmailService,EmailService>();
            //
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); 
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true; 
            });
            //
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
            .AddCookie(options =>
                {
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
                httpClient.BaseAddress = new Uri("https://localhost:7186/api/");
            });

            builder.Services.Configure<AuthTokenOptions>(
                builder.Configuration.GetSection("AuthTokenOptions")
            );

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseStatusCodePagesWithReExecute("/Error/NotFound");
            app.UseHttpsRedirection();
            
            app.UseStaticFiles();

            app.UseSession(); //use session

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
                else if (response.StatusCode == 404)
                {
                    response.Redirect("/Error/NotFound");
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
