using DatabaseSampler.Shared;

var builder = DistributedApplication.CreateBuilder(args);

var cosmosDatabaseIdParameter = builder.AddParameter(ParameterNames.CosmosDBDatabaseId);
var cosmosExpenseCollectionId = builder.AddParameter(ParameterNames.CosmosDBExpenseCollectionId);
var pgPasswordParameter = builder.AddParameter(ParameterNames.PostgreSQLPassword, secret: true);
var sqlPasswordParameter = builder.AddParameter(ParameterNames.SqlServerPassword, secret: true);
//var sqlPortParameter = builder.AddParameter(ParameterNames.SqlServerPort);

/*
Need
 - redis
 - sql db
    - initial migration to create db. Currently the application also runs migration
 - postgres db
    - initial script to create tables
 - cosmos db - UseEmulator

 - web site
    uses redis, postgres, sql db, cosmos db

 - function app
    uses postgres

Nuget packages for host
    Aspire.Hosting.Azure.CosmosDB
    Aspire.Hosting.Azure.Functions
    Aspire.Hosting.Redis
    Aspire.Hosting.PostgreSQL
    Aspire.Hosting.SqlServer
Nuget packages for website
    Aspire.Hosting.Redis
    Aspire.Data.EntityFramework.SqlServer
    not this Aspire.Microsoft.EntityFrameworkCore.Cosmos
    use Aspire.Microsoft.Azure.Cosmos
    Aspire.Microsoft.EntityFrameworkCore.SqlServer
    Aspire.Npgsql.EntityFrameworkCore.PostgreSQL
    
Aspire.Data.CosmosDb

Nuget packages for function app
    Aspire.Npgsql.EntityFrameworkCore.PostgreSQL

Links
    [Redis integration](https://aspire.dev/integrations/caching/redis/)
    [Aspire PostgreSQL Entity Framework Core integration](https://aspire.dev/integrations/databases/efcore/postgresql/)
    [Azure Cosmos DB integration](https://aspire.dev/integrations/cloud/azure/azure-cosmos-db/#hosting-integration)
    [Aspire.Microsoft.Azure.Cosmos](https://www.nuget.org/packages/Aspire.Microsoft.Azure.Cosmos/)
    [Aspire SQL Server Entity Framework Core integration](https://aspire.dev/integrations/databases/efcore/sql-server/)
    []()
 */
//var username = builder.AddParameter("username", secret: true);
//var password = builder.AddParameter("password", secret: true);

var cosmosDatabaseId = await cosmosDatabaseIdParameter.Resource.GetValueAsync(default).ConfigureAwait(true);
var expenseCollectionId = await cosmosExpenseCollectionId.Resource.GetValueAsync(default).ConfigureAwait(true);
var partitionKey = "/expense/name";

#pragma warning disable ASPIRECOSMOSDB001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var cosmos = builder.AddAzureCosmosDB(ResourceNames.CosmosDB)
  .RunAsPreviewEmulator(emulator =>
  //.RunAsEmulator(emulator =>
  {
      emulator.WithDataExplorer();
      emulator.WithDataVolume();
      emulator.WithLifetime(ContainerLifetime.Persistent);
  });

cosmos
    .AddCosmosDatabase(cosmosDatabaseId)
    .AddContainer(expenseCollectionId, partitionKey);

//Errors... pgcosmos readiness check still waiting for Postgres startu
//https://github.com/Azure/azure-cosmos-db-emulator-docker/issues/209
#pragma warning restore ASPIRECOSMOSDB001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

//var cosmosDatabase = cosmos.AddCosmosDatabase(cosmosDatabaseId);
//var expenseCollection = cosmosDatabase.AddContainer(expenseCollectionId, partitionKey);

var postgresdb = builder.AddPostgres(ResourceNames.Postgres)
    .WithPassword(pgPasswordParameter)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin()
    .AddDatabase(ResourceNames.PostgresDB, DatabaseNames.Students);

var sqldb = builder.AddSqlServer(ResourceNames.SqlServer)
    //.WithImage("mssql/server", "2025-latest")
    .WithDataVolume()
    .WithPassword(sqlPasswordParameter)
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase(ResourceNames.SqlDatabase, DatabaseNames.Locations);

var redis = builder.AddRedis(ResourceNames.Redis);

var postgresMigration = builder.AddProject<Projects.PostgresMigration>(ResourceNames.PostgresMigration)
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

var sqlServerDeployment = builder.AddProject<Projects.SqlServerDeployment>(ResourceNames.SqlServerDeployment)
    .WithReference(sqldb)
    .WaitFor(sqldb);

builder.AddAzureFunctionsProject<Projects.Functions>(ResourceNames.FunctionApp)
    .WithReference(postgresdb)
    .WaitForCompletion(postgresMigration);

builder.AddProject<Projects.Website>(ResourceNames.WebApp)
    .WithIconName("Globe")
    .WithReference(cosmos)
    .WithReference(redis)
    .WithReference(sqldb)
    .WithReference(postgresdb)
    .WithEnvironment($"{AppSettingNames.CosmosDBSection}:{AppSettingNames.CosmosDBDatabaseId}", cosmosDatabaseIdParameter)
    .WithEnvironment($"{AppSettingNames.CosmosDBSection}:{AppSettingNames.CosmosDBExpenseCollectionId}", cosmosExpenseCollectionId)
    .WaitFor(cosmos)
    .WaitForCompletion(postgresMigration)
    .WaitForCompletion(sqlServerDeployment);

await builder.Build().RunAsync().ConfigureAwait(true);
