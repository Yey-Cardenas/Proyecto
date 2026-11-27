using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Proyecto.Data;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var settingsDirectory = Path.Combine(currentDirectory, "Proyecto");
        if (!File.Exists(Path.Combine(settingsDirectory, "appsettings.Development.json")))
        {
            settingsDirectory = currentDirectory;
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(settingsDirectory)
            .AddJsonFile("appsettings.Development.json", optional: false)
            .Build();
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured for design-time operations.");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
