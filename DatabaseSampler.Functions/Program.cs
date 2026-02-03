using DatabaseSampler.Application.Data;
using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.Services;
using DatabaseSampler.Shared;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/*
var host = new HostBuilder()    
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(s =>
    {
        s.AddTransient<IPostgresSqlRepository, PostgresSqlRepository>();
        s.AddTransient<IPostgresSqlService, PostgresSqlService>();

        //Configure PostgresSql
        /*
        var pgConnectionString = Environment.GetEnvironmentVariable("PostgreSqlConnectionString");
        var pgPassword = Environment.GetEnvironmentVariable("PostgreSqlDbPassword");
        var pgBuilder = new NpgsqlConnectionStringBuilder(pgConnectionString)
        {
            Password = pgPassword
        };

        s.AddDbContext<StudentDbContext>(options =>
            options.UseNpgsql(pgBuilder.ConnectionString));
        */ /*
        //builder.AddNpgsqlDbContext<StudentDbContext>(connectionName: ResourceNames.PostgresDB);
        //s.AddDbContext<StudentDbContext>(options =>
        //    options.UseNpgsql(builder.Configuration.GetConnectionString("postgresdb")
        //        ?? throw new InvalidOperationException("Connection string 'postgresdb' not found.")));

        Console.WriteLine("Adding HTTP Client");
        s.AddHttpClient<ILocationService, LocationService>();

        s.AddTransient<ILocationService, LocationService>();
    })
    //.AddNpgsqlDbContext<StudentDbContext>(connectionName: ResourceNames.PostgresDB)
    .Build();
*/

var builder = FunctionsApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<StudentDbContext>(connectionName: ResourceNames.PostgresDB);

//TODO: Do this here or in Extensions?
builder.Services
    .AddTransient<IPostgresSqlRepository, PostgresSqlRepository>()
    .AddTransient<IPostgresSqlService, PostgresSqlService>()
    .AddTransient<ILocationService, LocationService>()
    ;

//TODO: Is location service also needed above?
builder.Services
    .AddHttpClient<ILocationService, LocationService>();

await builder.Build().RunAsync().ConfigureAwait(true);

