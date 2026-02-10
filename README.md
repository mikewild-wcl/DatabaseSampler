# Database Samples #

A site with samples for multiple databases. The solution uses Aspire, so all dependencies are defined in the AppHost project.

## Testing

Tests use **xUnit** + **Shouldly**.

Run all tests:

```
dotnet test
```

Only a few parameters are needed in the AppHost `appsettings.json` to run on a local machine:

> Note: do not commit secrets. Sensitive values like passwords should be set in the user secrets file.

```
  "Parameters": {
    "CosmosDBDatabaseId": "ToDoList",
    "CosmosDBExpenseCollectionId": "Items",
    "PostgreSQLPassword": "",
    "SqlServerPassword": ""
  },
```

> Note: package versions are managed centrally in `Directory.Packages.props`.

## Database migrations

Databases are created in AppHost using Aspire. The PostgreSQL database uses EF Core migrations, and the SQL Server database uses DbUp initialisation - these are both done in separate projects run at startup (or run in CI/CD provisioning for cloud scenarios.)

### Migration - using Package Manager
Make sure the main web project is set as the startup project, and in the Package Manager Console set the Default Project to DatabaseSampler.Application. Run 
```
Add-Migration InitialStudentEntities -Context DatabaseSampler.Application.Data.StudentDbContext 
Update-Database -Context DatabaseSampler.Application.Data.StudentDbContext
```

### Using dotnet ef tools:

See [Getting Started with EF Core](https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli)

Install with :
```
dotnet tool install --global dotnet-ef
```
or update with 
```
dotnet tool update --global dotnet-ef
```

Make sure the project has a reference to Microsoft.EntityFrameworkCore.Design and Microsoft.EntityFrameworkCore.Tools packages, and that the main web project is set as the startup project. Then run the following commands from the project directory:
To set up the initial migration
and update the database, run the following commands from the project directory:
```
dotnet ef migrations add InitialCreate
```

You can update the database, directly with 
```
dotnet ef database update
```
but since we are using Aspire the update will be handled by the Migrations project.

## Functions setup

`local.settings.json` is part of the project and is checked into git. There is no need to add any values because Aspire will handle all environment settings.
```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "ASPNETCORE_URLS": "http://localhost:60809"
  },
  "Host": {
    "CORS": "*",
    "CORSCredentials": false
  }
}
```

## Logger messages

Log message event id ranges:
- 000-099: Application
- 100-199: Bogus
- 200-299: Cosmos DB
- 300-399: PostgreSQL
- 400-499: SQL Server
- 500-599: MongoDB
- 800-899: Azure Functions

## Articles

[Cosmos DB MVC](https://developer.okta.com/blog/2019/07/11/aspnet-azure-cosmosdb-tutorial)

[Cosmos DB .NET Core](https://jeremylindsayni.wordpress.com/2019/02/25/getting-started-with-azure-cosmos-db-and-net-core-part-1-installing-the-cosmos-emulator/)

[Official tutorial](https://docs.microsoft.com/en-us/azure/cosmos-db/sql-api-dotnet-application)

[Functions V3 Preview Setup](https://dev.to/azure/develop-azure-functions-using-net-core-3-0-gcm)

## Code Downloads

[Cosmos Book Code](https://github.com/PacktPublishing/Guide-to-NoSQL-with-Azure-Cosmos-DB)


## Azure ML

[First ML experiment with R - Azure Machine Learning](https://docs.microsoft.com/en-us/azure/machine-learning/service/tutorial-1st-r-experiment?WT.mc_id=Revolutions-blog-davidsmi)
[Install](https://azure.github.io/azureml-sdk-for-r/articles/installation.html)
remotes::install_github('https://github.com/Azure/azureml-sdk-for-r', INSTALL_opts=c("--no-multiarch"))

Deployment - see https://github.com/Microsoft/AKSDeploymentTutorial
 or updated https://github.com/microsoft/AKSDeploymentTutorialAML


## Developer setup

### Requirements

* [Docker for X](https://docs.docker.com/install/#supported-platforms)
* https://docs.docker.com/docker-for-windows/install/

### Functions (dotnet-isolated)

Endpoints:
 - GetLocation: [GET,POST] http://localhost:60809/api/GetLocation 
 - GetStudents: [GET,POST] http://localhost:60809/api/GetStudents

See
 - Announcement: 
    - https://techcommunity.microsoft.com/t5/apps-on-azure/net-on-azure-functions-roadmap/ba-p/2197916
	- https://docs.microsoft.com/en-gb/azure/azure-functions/dotnet-isolated-process-developer-howtos?pivots=development-environment-vscode&tabs=browser
	- https://docs.microsoft.com/en-gb/azure/azure-functions/dotnet-isolated-process-guide
 - Older references - these use pre-release code
   - https://codetraveler.io/2021/02/12/creating-azure-functions-using-net-5/
   - https://mattjameschampion.com/2020/12/23/so-you-want-to-run-azure-functions-using-net-5/


Install Azure Functions Core Tools:
See Azure Functions Core Tools | [Installing](https://github.com/Azure/azure-functions-core-tools/blob/v4.x/README.md#installing)
with npm:

```
npm i -g azure-functions-core-tools@4 --unsafe-perm true
```
or winget: 
```
winget install Microsoft.Azure.FunctionsCoreTools
https://learn.microsoft.com/en-gb/azure/azure-functions/functions-run-local?tabs=windows%2Cisolated-process%2Cnode-v4%2Cpython-v2%2Chttp-trigger%2Ccontainer-apps&pivots=programming-language-csharp#install-the-azure-functions-core-tools
```

Then run from the project directory using
```
func host start --verbose
```



