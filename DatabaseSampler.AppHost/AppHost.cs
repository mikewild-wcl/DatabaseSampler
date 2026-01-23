using DatabaseSampler.Shared;

var builder = DistributedApplication.CreateBuilder(args);

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
    Aspire.Microsoft.EntityFrameworkCore.Cosmos
    Aspire.Hosting.Redis
    Aspire.Data.EntityFramework.SqlServer
    Aspire.Npgsql.EntityFrameworkCore.PostgreSQL
    
Aspire.Data.CosmosDb

Nuget packages for function app
    Aspire.Npgsql.EntityFrameworkCore.PostgreSQL

Links
    [Redis integration](https://aspire.dev/integrations/caching/redis/)
    [Aspire PostgreSQL Entity Framework Core integration](https://aspire.dev/integrations/databases/efcore/postgresql/)
    [Azure Cosmos DB integration](https://aspire.dev/integrations/cloud/azure/azure-cosmos-db/#hosting-integration)
    []()
    []()
    []()
 */
//var username = builder.AddParameter("username", secret: true);
//var password = builder.AddParameter("password", secret: true);

#pragma warning disable ASPIRECOSMOSDB001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var cosmos = builder.AddAzureCosmosDB(ResourceNames.CosmosDB)
  .RunAsPreviewEmulator(emulator =>
  {
      emulator.WithDataExplorer();
      emulator.WithDataVolume();
      emulator.WithLifetime(ContainerLifetime.Persistent);
  });
#pragma warning restore ASPIRECOSMOSDB001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var postgresdb = builder.AddPostgres(ResourceNames.Postgres)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase(ResourceNames.PostgresDB, DatabaseNames.Students);

var sqldb = builder.AddSqlServer(ResourceNames.SqlServer)
    //.WithImage("mssql/server", "2025-latest")
    .WithDataVolume()
    //.WithPassword(sqlPasswordParameter)
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase(ResourceNames.SqlDatabase, DatabaseNames.Locations);

var redis = builder.AddRedis(ResourceNames.Redis);

builder.AddAzureFunctionsProject<Projects.Functions>(ResourceNames.FunctionApp)
    .WithReference(postgresdb);

builder.AddProject<Projects.Website>(ResourceNames.WebApp)
    .WithReference(cosmos)
    .WithReference(redis)
    .WithReference(sqldb)
    .WithReference(postgresdb);

await builder.Build().RunAsync().ConfigureAwait(true);
