using Cofoundry.Domain.Data;

namespace SPASite.Data;

/// <summary>
/// This is a code-first EF DbContext that uses a handful of Cofoundry helpers
/// to make setting it up a bit easier. You can of course do data access any way you like.
/// 
/// See https://www.cofoundry.org/docs/framework/entity-framework-and-dbcontext-tools
/// </summary>
public class SPASiteDbContext : DbContext
{
    private readonly DatabaseSettings _databaseSettings;

    public SPASiteDbContext(DatabaseSettings databaseSettings)
    {
        _databaseSettings = databaseSettings;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_databaseSettings.ConnectionString);
    }

    /// <summary>
    /// We use the Cofoundry suggested schema config here which makes "app" the default schema.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAppSchema()
            .ApplyConfiguration(new CatLikeMap())
            .ApplyConfiguration(new CatLikeCountMap());
    }

    public DbSet<CatLike> CatLikes { get; set; }
    public DbSet<CatLikeCount> CatLikeCounts { get; set; }
}
