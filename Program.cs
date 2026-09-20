using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AprilBookStore.Models;
using AprilBookStore.DataAccess;
using NuGet.Packaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using AprilBookStore.Security;
using System.Security.Principal;
namespace AprilBookStore
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnections") ?? throw new InvalidOperationException("Connection string 'AprilBookStoreContextConnection' not found.");

            builder.Services.AddDbContext<BookStoreContext>(options =>
                options.UseSqlServer(connectionString)
                .EnableSensitiveDataLogging());
            
            builder.Services.AddIdentity<ApplicationUser,IdentityRole>(options => {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireNonAlphanumeric=false;
                options.User.RequireUniqueEmail = true;
            }).AddDefaultTokenProviders().AddDefaultUI().AddEntityFrameworkStores<BookStoreContext>();

            builder.Services.AddTransient<IData, DataContext>();
            builder.Services.AddTransient<IAuthorizationHandler, CanEditOnlyOthersRolesHandler>();
            builder.Services.AddTransient<IAuthorizationHandler, SuperAdminHandler>();

            var clientId = builder.Configuration["ClientId"];
            var clientSecret = builder.Configuration["ClientSecret"];
            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId=clientId;
                    options.ClientSecret=clientSecret;
                });
            builder.Services.AddAuthorization(options=> {
                options.AddPolicy("DeleteRolePolicy", p => p.RequireRole("SuperAdmin"));
                options.AddPolicy("EditRolePolicy", p => p.AddRequirements(new RoleEditingRequirement()));
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
}
