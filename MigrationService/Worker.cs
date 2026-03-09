using GitViewerLogging.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace GitViewerLogging.MigrationService;
public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime lifetime) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LoggingContext>();
        await db.Database.MigrateAsync(cancellationToken);

        lifetime.StopApplication();
    }
}
