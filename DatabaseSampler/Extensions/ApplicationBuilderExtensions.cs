using DatabaseSampler.Application.Configuration;
using DatabaseSampler.Application.Data;
using DatabaseSampler.Application.DataGenerator;
using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.Services;
using DatabaseSampler.Shared;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace DatabaseSampler.Extensions;

[SuppressMessage("Minor Code Smell", "S2325:Methods and properties that don't access instance data should be static", Justification = "Extension members don't need to be static")]
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
            //TODO: Do this here or in Program? It isn't one of our application services
            builder.Services.AddMemoryCache();

            builder.Services
                .AddTransient<IDataGenerator, BogusDataGenerator>()
                .AddTransient<ICosmosDbService, CosmosDbService>()
                .AddTransient<IPostgresSqlRepository, PostgresSqlRepository>()
                .AddTransient<IPostgresSqlService, PostgresSqlService>();

            //builder.services.AddHttpClient<ICosmosDbService, CosmosDbService>();

            return builder;
        }

        public IHostApplicationBuilder AddDatabases()
        {
            /*
            builder.AddSqlServerDbContext<LocationDbContext>(
                connectionName: ResourceNames.SqlDatabase, 
                //configureSettings: options => { 
                    //options.EnableRetryOnFailure = true;
                //},
                configureDbContextOptions: options =>
                {
                    //options.UseNetTopologySuite();
                    //options.EnableRetryOnFailure = true;
                })
                ;
            */

            builder.Services.AddDbContext<LocationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString(ResourceNames.SqlDatabase)
                                    ?? throw new InvalidOperationException(
                                        $"Connection string '{ResourceNames.SqlDatabase}' not found."),
                    options => options
                        .UseNetTopologySuite()
                        .EnableRetryOnFailure()),
                ServiceLifetime.Transient);

            /*
            builder.Services
                .AddDbContext<StudentDbContext>(options =>
                    options.UseNpgsql(pgBuilder.ConnectionString));
            */
            builder.AddNpgsqlDbContext<StudentDbContext>(connectionName: ResourceNames.PostgresDB);

            //builder.AddCosmosDbContext<MyDbContext>(ResourceNames.CosmosDB);
            builder.AddAzureCosmosClient(ResourceNames.CosmosDB);

            return builder;
        }

        public IHostApplicationBuilder AddHttpClients()
        {
            builder.Services
                .AddHttpClient<ILocationService, LocationService>();

            //builder.services.AddHttpClient<ICosmosDbService, CosmosDbService>();

            return builder;
        }
    }
}
