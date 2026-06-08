using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JodWai.Infrastructure.Persistence.Workers;

internal sealed class MigrationWorker(
    IServiceProvider services,
    IHostApplicationLifetime lifetime,
    ILogger<MigrationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        try
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider
                          .GetRequiredService<AppDbContext>();

            await db.Database.MigrateAsync(ct);

            logger.LogInformation("Database migration completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Database migration failed.");
            throw; // Let the host crash — do not silently swallow
        }
        finally
        {
            lifetime.StopApplication();
        }
    }
}
