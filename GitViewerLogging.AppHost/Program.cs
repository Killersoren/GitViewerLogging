using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("postgresPassword", "1234");


// Add a PostgreSQL container
var postgres = builder.AddPostgres("postgresLogging")
    .WithDataVolume()
    .WithPassword(password)
    .WithEnvironment("POSTGRES_PASSWORD", "1234")
    .WithHostPort(5433)
    .WithPgAdmin();


var migrations = builder.AddProject<GitViewerLogging_MigrationService>("migrations")
        .WithReference(postgres)
        .WaitFor(postgres);

var loggingService = builder.AddProject<GitViewerLogging_Api>("GitViewerloggingApi")
    .WithReference(postgres)
    .WithReference(migrations)
    .WaitFor(migrations);


await builder.Build().RunAsync();
