using Microsoft.EntityFrameworkCore;

namespace Monte.WebApi;

public static class Setup
{
    public static async Task MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MonteDbContext>();

        await context.Database.MigrateAsync();
    }
}