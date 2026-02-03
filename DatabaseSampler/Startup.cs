using System.Net;
using DatabaseSampler.Application.Configuration;
using DatabaseSampler.Application.Data;
using DatabaseSampler.Application.DataGenerator;
using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.Services;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DatabaseSampler;

/*
public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        //services.AddMvc();
        services.AddControllersWithViews();

        services.AddMemoryCache();

        //Configure Redis distributed cache
        //var redisConnectionString = Configuration["RedisConnectionString"];
        //services.AddStackExchangeRedisCache(options => {
        //    options.Configuration = redisConnectionString;
        //    options.InstanceName = "";
        //});
        //services.AddRedisCache(appHost.GetRedisConnectionString());
        //TODO: builder.AddRedisClient(connectionName: "cache");

        services.AddTransient<IDataGenerator, BogusDataGenerator>();

        services.AddHttpClient<ILocationService, LocationService>();

        //Configure CosmosDB
        var cosmosConfig = new CosmosDbConfiguration();
        Configuration.Bind("CosmosConfig", cosmosConfig);
        services.AddSingleton(cosmosConfig);

        try
        {
            services.AddHttpClient<ICosmosDbService, CosmosDbService>();
            services.AddSingleton<ICosmosDbService>(
                InitializeCosmosClientInstanceAsync(cosmosConfig)
                    .GetAwaiter().GetResult());
        }
        catch (Exception ex)
        {
            LogException(ex, "Could not start CosmosDB services");
        }

        //Configure PostgresSQL
        services.AddTransient<IPostgresSqlRepository, PostgresSqlRepository>();
        services.AddTransient<IPostgresSqlService, PostgresSqlService>();

        //Configure SQL Server
        var sqlConnectionString = Configuration["SqlConnectionString"];
        services.AddDbContext<LocationDbContext>(options =>
            options.UseSqlServer(sqlConnectionString, builder => builder
                .UseNetTopologySuite()
                .EnableRetryOnFailure()),
            ServiceLifetime.Transient);

        //Configure PostgresSql
        var pgConnectionString = Configuration["PostgreSql:ConnectionString"];
        var pgPassword = Configuration["PostgreSql:DbPassword"];
        var pgBuilder = new NpgsqlConnectionStringBuilder(pgConnectionString)
        {
            Password = pgPassword
        };

        services.AddDbContext<StudentDbContext>(options =>
            options.UseNpgsql(pgBuilder.ConnectionString));

        //Configure Azure Search
        //AzureSearchConfiguration

        var azureConfig = new AzureSearchConfiguration();
        Configuration.Bind("AzureSearchConfiguration", azureConfig);
        services.AddSingleton(azureConfig);
        //var azureConfig = Configuration["AzureSearchConfiguration"];
    }

    private async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(CosmosDbConfiguration config)
    {
        //var databaseName = Configuration["CosmosConfig:DatabaseId"];
        //var containerName = Configuration["CosmosConfig:ExpenseCollectionId"];
        //var account = Configuration["CosmosConfig:EndpointUri"];
        //var key = Configuration["CosmosConfig:AuthorizationKey"];

        var clientBuilder = new CosmosClientBuilder(config.EndpointUri, config.AuthorizationKey);
        var client = clientBuilder
            .WithConnectionModeDirect()
            .Build();

        var cosmosDbService = new CosmosDbService(client, config.DatabaseId, config.ExpenseCollectionId);
        var databaseResponse = await client.CreateDatabaseIfNotExistsAsync(config.DatabaseId);

        Console.WriteLine($@"Cosmos DB '{config.DatabaseId} {(databaseResponse.StatusCode == HttpStatusCode.Created ? "created" : "retrieved")}");
        Console.WriteLine($@"    - Etag '{databaseResponse.ETag}");
        Console.WriteLine($@"    - ActivityId {databaseResponse.ActivityId}");
        Console.WriteLine($@"    - Charge {databaseResponse.RequestCharge:#.00}");
        
        //TODO: Get a better partition name
        var partitionKey = "/expense/name";
        var containerResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(config.ExpenseCollectionId, partitionKey);

        Console.WriteLine($@"Cosmos container '{config.ExpenseCollectionId} {(containerResponse.StatusCode == HttpStatusCode.Created ? "created" : "retrieved")}");
        Console.WriteLine($@"    - Etag '{containerResponse.ETag}");
        Console.WriteLine($@"    - ActivityId {containerResponse.ActivityId}");
        Console.WriteLine($@"    - Charge {containerResponse.RequestCharge:#.00}");

        return cosmosDbService;
    }

    private void LogException(Exception exception, string message)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.WriteLine($"   {exception.Message}");
        Console.ForegroundColor = color;
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            //app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            //endpoints.MapControllers();
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}
*/
