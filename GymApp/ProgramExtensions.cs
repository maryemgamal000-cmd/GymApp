using GymApp;
using GymSystem.DAL.Data.DataSeeding;
using GymSystem.DAL.Data.DBContexts;
using Microsoft.EntityFrameworkCore;

namespace GymSystem.PL
{
    public static class ProgramExtensions
    {

        public async static Task MigrateAndSeedDatabaseAsync(this WebApplication app) 
        {

            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var pendingMigration = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigration.Any())
            {
                logger.LogInformation($"Applying {pendingMigration.Count()} Pending Migration");
                await dbContext.Database.MigrateAsync();
            }


            var seedFolderPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "Files");
            await GymDataSeeding.SeedAsync(dbContext, seedFolderPath, logger);

        }
    }
}
