using AprilBookStore.Application;
using AprilBookStore.Application.Interfaces;
using AprilBookStore.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace AprilBookStore.Web;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();
        builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);

        var clientId = builder.Configuration["ClientId"];
        var clientSecret = builder.Configuration["ClientSecret"];
        var authBuilder = builder.Services.AddAuthentication();
        if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(clientSecret))
        {
            authBuilder.AddGoogle(options =>
            {
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
            });
        }
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("DeleteRolePolicy", p => p.RequireRole("SuperAdmin"));
            options.AddPolicy("EditRolePolicy", p => p.RequireRole("SuperAdmin"));
        });
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
        }

        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        app.Run();
    }
}
