using DatabaseSampler.Application.Configuration;
using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.Models;
using DatabaseSampler.Shared;
using Microsoft.Azure.Cosmos;
using NetTopologySuite.Geometries.Utilities;
using System.Globalization;
using System.Reflection.Metadata;

namespace DatabaseSampler.Application.Services;

public class CosmosDbService(
    CosmosClient dbClient,
    CosmosDbConfiguration config) : ICosmosDbService
{
    private readonly CosmosClient _dbClient = dbClient;
    private readonly CosmosDbConfiguration _config = config;

    public async Task AddExpenseAsync(Expense expense)
    {
        ArgumentNullException.ThrowIfNull(expense);

        var container = _dbClient.GetContainer(_config.DatabaseId, _config.ExpenseCollectionId);
        var partition = !string.IsNullOrWhiteSpace(expense.Name)
            ? char.ToUpper(expense.Name[0], CultureInfo.InvariantCulture).ToString()
            : "EMPTY";

        var properties = await container.ReadContainerAsync();
        //Console.WriteLine(properties.Container..PartitionKeyPath);

        var response = await container.CreateItemAsync(
            new
            {
                id = expense.Id.ToString(),
                //partitionKey = $"{PartitionKeyPaths.ExpenseName}/{partition}",
                expense = new { name = expense.Name },
                expense.Name,
                expense.Amount,
                expense.Frequency,
                expense.StartDate,
                expense.MonthlyCost,
                expense.Created
            },
            //https://pmichaels.net/2021/03/13/cosmosdb-errors-on-inserting-new-data/
                  //new PartitionKey($"{PartitionKeyPaths.ExpenseName}/{partition}"));
            new PartitionKey(PartitionKeyPaths.ExpenseName));
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

    //private static async Task<Offer> UpdateOfferForCollectionAsync(string collectionSelfLink, int newOfferThroughput)
    //{
    //    // Create an asynchronous query to retrieve the current offer for the document collection
    //    // Notice that the current version of the API only allows to use the SelfLink for the collection 
    //    // to retrieve its associated offer
    //    Offer existingOffer = null;
    //    var offerQuery = client.CreateOfferQuery()
    //        .Where(o => o.ResourceLink == collectionSelfLink)
    //        .AsDocumentQuery();
    //    while (offerQuery.HasMoreResults)
    //    {
    //        foreach (var offer in await offerQuery.ExecuteNextAsync<Offer>())
    //        {
    //            existingOffer = offer;
    //        }
    //    }
    //    if (existingOffer == null)
    //    {
    //        throw new Exception("I couldn't retrieve the offer for the collection.");
    //    }
    //    // Set the desired throughput to newOfferThroughput RU/s for the new offer built based on the current offer
    //    var newOffer = new OfferV2(existingOffer, newOfferThroughput);
    //    var replaceOfferResponse = await client.ReplaceOfferAsync(newOffer);

    //    return replaceOfferResponse.Resource;
    //}    
}
