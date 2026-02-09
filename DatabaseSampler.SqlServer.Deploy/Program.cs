using DatabaseSampler.Shared;
using DatabaseSampler.SqlServer.Deploy.Logging;
using DbUp;
using DbUp.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

var builder = Host.CreateApplicationBuilder();
builder.AddServiceDefaults();

using var loggerFactory = LoggerFactory.Create(builder => builder.AddSimpleConsole());
var logger = loggerFactory.CreateLogger<Program>();

var connectionStrings = builder.Configuration.GetSection("ConnectionStrings");
foreach (var item in connectionStrings.GetChildren())
{
    System.Diagnostics.Debug.WriteLine($"Connection string {item.Key}={item.Value}");
}

var connectionString = builder.Configuration.GetConnectionString(ResourceNames.SqlDatabase);

var serviceProvider = builder.Build().Services;

Log.StartingSqlServerDeployment(logger);

var result = DeployChanges.To
    .SqlDatabase(connectionString)
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .WithTransaction()
    .AddLoggerFromServiceProvider(serviceProvider)
    .Build()
    .PerformUpgrade();

if (!result.Successful)
{
    Log.DeploymentFailed(logger, result.Error!);
    return -1;
}

Log.DeployedSqlServerSuccessfully(logger);

return 0;
