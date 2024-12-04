using GithubRepositoryAnalyzer.Config.Options;
using GithubRepositoryAnalyzer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GithubRepositoryAnalyzer.Domain.Context;

public class ConfigContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public DbSet<UserToken> UserTokens { get; set; }

    private readonly ConnectionSettings ConnectionSettings;

    public ConfigContext(IOptions<ConnectionSettings> connectionSettings)
    {
        ConnectionSettings = connectionSettings.Value;
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfiguration(new Configurations.UserConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.UserTokenConfiguration());

        base.OnModelCreating(modelBuilder);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder is not { IsConfigured: false })
        {
            return;
        }

        optionsBuilder.UseNpgsql(
            ConnectionSettings.DatabaseConnection,
            builder =>
            {
                builder.MigrationsAssembly("GithubRepositoryAnalyzer.Domain");
            });
    }
    
    /// <inheritdoc />
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyChanges();

        return base.SaveChangesAsync(cancellationToken);
    }
    
    private void ApplyChanges()
    {
        ApplyChangesToEntries<User>();
        ApplyChangesToEntries<UserToken>();
    }
    
    private void ApplyChangesToEntries<T>()
        where T : Entity
    {
        var addedOrModifiedEntries = ChangeTracker.Entries<T>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in addedOrModifiedEntries)
        {
            entry.Entity.RecalculateIds();
        }
    }
}