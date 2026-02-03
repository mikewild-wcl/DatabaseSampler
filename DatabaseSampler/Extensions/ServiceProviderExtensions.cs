using DatabaseSampler.Application.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace DatabaseSampler.Extensions;

//[SuppressMessage("Minor Code Smell", "S2325:Methods and properties that don't access instance data should be static", Justification = "Extension members don't need to be static")]
internal static class ServiceProviderExtensions
{
    extension(IServiceProvider sp)
    {
        [SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "Logger only called at startup")]
        public async Task InitializeCosmosDb()
        {
            const string partitionKey = "/expense/name";

            var config = sp.GetRequiredService<CosmosDbConfiguration>();
            var client = sp.GetRequiredService<CosmosClient>();
            var logger = sp.GetRequiredService<ILogger<Program>>();

            var databaseResponse = await client.CreateDatabaseIfNotExistsAsync(config.DatabaseId).ConfigureAwait(true);

            logger.LogInformation("Cosmos DB '{DatabaseId} {Action}'",
                config.DatabaseId,
                databaseResponse.StatusCode == System.Net.HttpStatusCode.Created ? "created" : "retrieved");

            var containerResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(config.ExpenseCollectionId, partitionKey).ConfigureAwait(true);

            logger.LogInformation("Cosmos DB container '{ContainerId} {Action}'",
                containerResponse.Container.Id,
                containerResponse.StatusCode == System.Net.HttpStatusCode.Created ? "created" : "retrieved");
        }
    }
}
