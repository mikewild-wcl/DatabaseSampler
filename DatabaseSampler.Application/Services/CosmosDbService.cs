using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.Models;
using Microsoft.Azure.Cosmos;

namespace DatabaseSampler.Application.Services;

public class CosmosDbService : ICosmosDbService
{
    private readonly Container _container;

    public CosmosDbService(
        CosmosClient dbClient,
        string databaseName,
        string containerName)
    {
        _container = dbClient.GetContainer(databaseName, containerName);
    }

    public async Task<IList<Expense>> GetItemsAsync(string queryString)
    {
        var query = _container.GetItemQueryIterator<Expense>(new QueryDefinition(queryString));
        var results = new List<Expense>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();

            results.AddRange(response.ToList());
        }

        return results;

        //GetItemsAsync("SELECT * FROM c");

        //var client = new CosmosClient("x");
        //var container = client.GetContainer("databaseid", "container");
        //var db = client.GetDatabase("databaseid");
        //db..
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
