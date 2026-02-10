using DatabaseSampler.Shared;

var builder = DistributedApplication.CreateBuilder(args);

var cosmosDatabaseIdParameter = builder.AddParameter(ParameterNames.CosmosDBDatabaseId);
var cosmosExpenseCollectionId = builder.AddParameter(ParameterNames.CosmosDBExpenseCollectionId);
var pgPasswordParameter = builder.AddParameter(ParameterNames.PostgreSQLPassword, secret: true);
var sqlPasswordParameter = builder.AddParameter(ParameterNames.SqlServerPassword, secret: true);
//TODO: Use SQL Parameter?
//var sqlPortParameter = builder.AddParameter(ParameterNames.SqlServerPort);

var cosmosDatabaseId = await cosmosDatabaseIdParameter.Resource.GetValueAsync(default).ConfigureAwait(true);
var expenseCollectionId = await cosmosExpenseCollectionId.Resource.GetValueAsync(default).ConfigureAwait(true);

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
    .AddContainer(expenseCollectionId, PartitionKeyPaths.ExpenseName);

//Errors... pgcosmos readiness check still waiting for Postgres startup
//https://github.com/Azure/azure-cosmos-db-emulator-docker/issues/209
#pragma warning restore ASPIRECOSMOSDB001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var mongodb = builder.AddMongoDB(ResourceNames.Mongo)
    .WithMongoExpress()
    .WithDataVolume(ResourceNames.MongoDataVolume)
    .AddDatabase(ResourceNames.MongoDB, databaseName: DatabaseNames.Teachers);

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
    .WithReference(mongodb)
    .WithReference(postgresdb)
    .WithReference(redis)
    .WithReference(sqldb)
    .WithEnvironment($"{AppSettingNames.CosmosDBSection}:{AppSettingNames.CosmosDBDatabaseId}", cosmosDatabaseIdParameter)
    .WithEnvironment($"{AppSettingNames.CosmosDBSection}:{AppSettingNames.CosmosDBExpenseCollectionId}", cosmosExpenseCollectionId)
    .WaitFor(cosmos)
    .WaitFor(mongodb)
    .WaitForCompletion(postgresMigration)
    .WaitForCompletion(sqlServerDeployment);

await builder.Build().RunAsync().ConfigureAwait(true);
