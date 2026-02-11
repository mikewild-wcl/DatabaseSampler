using DatabaseSampler.Application.Configuration;
using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DatabaseSampler.Application.Services;

public partial class CosmosDbService(
    CosmosClient dbClient,
    CosmosDbConfiguration config,
    ILogger<CosmosDbService> logger) : ICosmosDbService
{
    private readonly CosmosClient _dbClient = dbClient;
    private readonly CosmosDbConfiguration _config = config;
    private readonly ILogger<CosmosDbService> _logger = logger;

    [LoggerMessage(
        EventId = 211,
        Level = LogLevel.Information,
        Message = "Expense created with Id {Id} Name {Name}, Amount {Amount}, Response status {StatusCode}.")]
    private static partial void LogExpenseCreated(ILogger logger, Guid Id, string name, decimal amount, HttpStatusCode statusCode);

    public async Task AddExpenseAsync(Expense expense)
    {
        ArgumentNullException.ThrowIfNull(expense);

        var container = _dbClient.GetContainer(_config.DatabaseId, _config.ExpenseCollectionId);

        var response = await container.CreateItemAsync(
            expense,
            new PartitionKey(expense.Name));

        LogExpenseCreated(_logger, expense.Id, expense.Name, expense.Amount, response.StatusCode);
    }

    public async Task<IList<Expense>> GetItemsAsync(string queryString)
    {
        var container = _dbClient.GetContainer(_config.DatabaseId, _config.ExpenseCollectionId);

        var query = container.GetItemQueryIterator<Expense>(new QueryDefinition(queryString));
        var results = new List<Expense>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();

            results.AddRange(response.ToList());
        }

        return results;
    }
}
