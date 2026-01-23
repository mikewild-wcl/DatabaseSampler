using DatabaseSampler.Application.Data;
using DatabaseSampler.Application.Interfaces;
using DatabaseSampler.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

var host = new HostBuilder()    
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(s =>
    {
        s.AddTransient<IPostgresSqlRepository, PostgresSqlRepository>();
        s.AddTransient<IPostgresSqlService, PostgresSqlService>();

        //Configure PostgresSql
        var pgConnectionString = Environment.GetEnvironmentVariable("PostgreSqlConnectionString");
        var pgPassword = Environment.GetEnvironmentVariable("PostgreSqlDbPassword");
        var pgBuilder = new NpgsqlConnectionStringBuilder(pgConnectionString)
        {
            Password = pgPassword
        };

        s.AddDbContext<StudentDbContext>(options =>
            options.UseNpgsql(pgBuilder.ConnectionString));

        Console.WriteLine("Adding HTTP Client");
        s.AddHttpClient<ILocationService, LocationService>();

        s.AddTransient<ILocationService, LocationService>();
    })
    .Build();

await host.RunAsync();
