using DatabaseSampler.Application.Configuration;
using DatabaseSampler.Application.Data;
using DatabaseSampler.Application.DataGenerator;
using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.Services;
using DatabaseSampler.Shared;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSampler.Extensions;

internal static class ApplicationBuilderExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder AddConfiguration()
        {
            var cosmosConfig = new CosmosDbConfiguration();
            builder.Configuration.Bind("CosmosConfig", cosmosConfig);
            builder.Services.AddSingleton(cosmosConfig);

            return builder;
        }

        public IHostApplicationBuilder AddServices()
        {
            builder.Services
                .AddTransient<IDataGenerator, BogusDataGenerator>()
                .AddTransient<ICosmosDbService, CosmosDbService>()
                .AddTransient<IMongoDbService, MongoDbService>()
                .AddTransient<IPostgresSqlRepository, PostgresSqlRepository>()
                .AddTransient<IPostgresSqlService, PostgresSqlService>();

            return builder;
        }

        public IHostApplicationBuilder AddDatabases()
        {
            builder.Services.AddDbContext<LocationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString(ResourceNames.SqlDatabase)
                                    ?? throw new InvalidOperationException(
                                        $"Connection string '{ResourceNames.SqlDatabase}' not found."),
                    options => options
                        .UseNetTopologySuite()
                        .EnableRetryOnFailure()),
                ServiceLifetime.Transient);

            builder.AddMongoDBClient(ResourceNames.MongoDB);

            builder.AddNpgsqlDbContext<StudentDbContext>(connectionName: ResourceNames.PostgresDB);

            builder.AddAzureCosmosClient(ResourceNames.CosmosDB, 
                configureClientOptions: options =>
                    options.SerializerOptions = new CosmosSerializationOptions()
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                    });

            return builder;
        }

        public IHostApplicationBuilder AddHttpClients()
        {
            builder.Services
                .AddHttpClient<ILocationService, LocationService>(client =>
                {
                    client.BaseAddress = new(LocationService.PostcodeRetrieverBaseUrl);
                });

            return builder;
        }
    }
}
