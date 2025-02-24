using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
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
            builder.Services.AddScoped<TicketServices>();
            builder.Services.AddScoped<UserServices>();

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
            app.UseStatusCodePagesWithReExecute("/Error/NotFound");
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
