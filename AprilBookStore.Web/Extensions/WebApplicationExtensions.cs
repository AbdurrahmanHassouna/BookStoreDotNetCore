using AprilBookStore.Infrastructure.Data;

namespace AprilBookStore.Web.Extensions;

public static class WebApplicationExtensions
{
    public static async Task<WebApplication> SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            await DbInitializer.SeedIdentityAsync(services, app.Configuration);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<WebApplication>>();
            logger.LogError(ex, "An error occurred while seeding Identity data.");
        }

        return app;
    }
}
