using DatabaseSampler.Application.Data;
using DatabaseSampler.Postgres.MigrationService;
using DatabaseSampler.Shared;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddNpgsqlDbContext<StudentDbContext>(connectionName: ResourceNames.PostgresDB);

var host = builder.Build();
await host.RunAsync().ConfigureAwait(true);
