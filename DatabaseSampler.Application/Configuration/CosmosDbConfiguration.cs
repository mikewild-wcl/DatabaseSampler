
namespace DatabaseSampler.Application.Configuration;

public class CosmosDbConfiguration
{
    public string EndpointUri { get; set; }

    public string AuthorizationKey { get; set; }

    public string DatabaseId { get; set; }

    public string ExpenseCollectionId { get; set; }
}
