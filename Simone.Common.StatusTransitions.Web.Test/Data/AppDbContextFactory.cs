using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Simone.Common.StatusTransitions.Web.Test.Data
{
    /// <summary>
    /// Used by the EF Core tools (<c>dotnet ef</c>) at design time so the migration commands
    /// build the context directly, without running the web host (and its startup Migrate/Seed).
    /// </summary>
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("Database"));

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
