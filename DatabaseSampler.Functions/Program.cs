using DatabaseSampler.Application.Data;
using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.Services;
using DatabaseSampler.Shared;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.ConfigureFunctionsWebApplication();

builder.AddNpgsqlDbContext<StudentDbContext>(connectionName: ResourceNames.PostgresDB);

builder.Services
    .AddTransient<IPostgresSqlRepository, PostgresSqlRepository>()
    .AddTransient<IPostgresSqlService, PostgresSqlService>();

builder.Services
    .AddHttpClient<ILocationService, LocationService>();

await builder.Build().RunAsync().ConfigureAwait(true);
