using Microsoft.EntityFrameworkCore;
using StarWarsProgressBarIssueTracker.Infrastructure.Database;
using StarWarsProgressBarIssueTracker.Infrastructure.Models;

namespace StarWarsProgressBarIssueTracker.App.Extensions;

public static class DbContextExtensions
{
    public static void RegisterDbContext(this IServiceCollection services, string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentOutOfRangeException(nameof(connectionString), "The ConnectionString should not be null, empty or whitespace.");
        }

        services.AddDbContext<IssueTrackerContext>(optionsBuilder => optionsBuilder.UseNpgsql(connectionString));
    }

    public static async Task ConfigureDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<IssueTrackerContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            await ApplyMigrationsAsync(context, logger);

            await SeedDatabaseAsync(context, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured during the database migration check.");
        }
    }

    private static async Task ApplyMigrationsAsync(IssueTrackerContext context, ILogger logger)
    {
        bool needMigration = (await context.Database.GetPendingMigrationsAsync()).Any();

        if (needMigration)
        {
            logger.LogInformation("The database is not up-to-date. Applying automatic pending migrations.");
            await context.Database.MigrateAsync();
        }
        else
        {
            logger.LogInformation("The database is up-to-date. No migration needed.");
        }
    }

    private static async Task SeedDatabaseAsync(IssueTrackerContext context, ILogger logger)
    {
        try
        {
            if (await context.Jobs.AnyAsync())
            {
                logger.LogInformation("You can only seed an empty database. No data was added.");
                return;
            }

            await context.Jobs.AddAsync(new DbJob
            {
                CronInterval = "0 0/1 * * * ?",
                IsPaused = false,
                JobType = JobType.GitlabSync,
            });
            await context.SaveChangesAsync();
            // TODO: seed e.g. jobs
        }
        catch (DbUpdateException ex)
        {
            logger.LogWarning(ex, "Could not seed database as requested.");
        }
    }
}
