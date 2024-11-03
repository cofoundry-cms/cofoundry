using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;

namespace Cofoundry.Plugins.ErrorLogging.Data;

public partial class ErrorLoggingDbContext : DbContext
{
    private readonly ICofoundryDbContextInitializer _cofoundryDbContextInitializer;

    public ErrorLoggingDbContext(
        ICofoundryDbContextInitializer cofoundryDbContextInitializer
        )
    {
        _cofoundryDbContextInitializer = cofoundryDbContextInitializer;
    }

    public DbSet<Error> Errors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        _cofoundryDbContextInitializer.Configure(this, optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema(DbConstants.CofoundryPluginSchema)
            .ApplyConfiguration(new ErrorMap());
    }
}
