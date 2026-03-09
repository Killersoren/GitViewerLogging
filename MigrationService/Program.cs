using GitViewerLogging.DataAccess.Models;

namespace GitViewerLogging.MigrationService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.AddServiceDefaults();

        builder.AddNpgsqlDbContext<LoggingContext>("pgdata");
        builder.Services.AddHostedService<Worker>();

        var app = builder.Build();
        app.Run();
    }
}