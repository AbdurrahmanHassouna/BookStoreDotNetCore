using AprilBookStore.Application.Interfaces;
using AprilBookStore.Domain.Entities;
using AprilBookStore.Infrastructure.Data;
using AprilBookStore.Infrastructure.Data.Interceptors;
using AprilBookStore.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AprilBookStore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnections")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnections' not found.");

        services.AddScoped<AuditableEntityInterceptor>();

        services.AddDbContext<BookStoreContext>((sp, options) =>
            options.UseSqlServer(connectionString, optionsBuilder => optionsBuilder.MigrationsAssembly("AprilBookStore.Infrastructure"))
                   .AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>())
                   .EnableSensitiveDataLogging());

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<BookStoreContext>());

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddDefaultTokenProviders()
        .AddDefaultUI()
        .AddEntityFrameworkStores<BookStoreContext>();

        services.AddScoped<IFileStorageService, FileStorageService>();

        return services;
    }
}
