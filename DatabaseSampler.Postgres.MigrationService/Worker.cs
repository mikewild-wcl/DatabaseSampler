using DatabaseSampler.Application.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;

namespace DatabaseSampler.Postgres.MigrationService;

internal class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource _activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = _activitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StudentDbContext>();

            //await EnsureDatabaseAsync(dbContext, stoppingToken);
            await RunMigrationAsync(dbContext, stoppingToken);
            await SeedDataAsync(dbContext, stoppingToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task EnsureDatabaseAsync(StudentDbContext dbContext, CancellationToken cancellationToken)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Create the database if it does not exist.
            // Do this first so there is then a database to start a transaction against.
            if (!await dbCreator.ExistsAsync(cancellationToken))
            {
                await dbCreator.CreateAsync(cancellationToken);
            }
        });
    }

    private static async Task RunMigrationAsync(StudentDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }

    private static async Task SeedDataAsync(StudentDbContext dbContext, CancellationToken cancellationToken)
    {
        //SupportTicket firstTicket = new()
        //{
        //    Title = "Test Ticket",
        //    Description = "Default ticket, please ignore!",
        //    Completed = true
        //};

        //var strategy = dbContext.Database.CreateExecutionStrategy();
        //await strategy.ExecuteAsync(async () =>
        //{
        //    // Seed the database
        //    await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        //    await dbContext.Tickets.AddAsync(firstTicket, cancellationToken);
        //    await dbContext.SaveChangesAsync(cancellationToken);
        //    await transaction.CommitAsync(cancellationToken);
        //});
    }
}
