using Microsoft.Extensions.Logging;

namespace DatabaseSampler.SqlServer.Deploy.Logging;

internal static partial class Log
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Starting SQL Server deployment...")]
    public static partial void StartingSqlServerDeployment(ILogger logger);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Deployed SQL Server successfully!")]
    public static partial void DeployedSqlServerSuccessfully(ILogger logger);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "Deployment failed.")]
    public static partial void DeploymentFailed(ILogger logger, Exception exception);
}


