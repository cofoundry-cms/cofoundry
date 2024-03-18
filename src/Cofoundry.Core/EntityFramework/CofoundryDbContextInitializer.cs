﻿using Cofoundry.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cofoundry.Core.EntityFramework.Internal;

/// <summary>
/// Used to configure the default settings for an EF DbContext that connects 
/// to the Cofoundry database. This default implementation will set up the
/// logger and use the shared Cofoundry connection so that any DbContext
/// initialized this way can enlist in the same transaction.
/// </summary>
public class CofoundryDbContextInitializer : ICofoundryDbContextInitializer
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ICofoundryDbConnectionManager _cofoundryDbConnectionFactory;

    public CofoundryDbContextInitializer(
        ILoggerFactory loggerFactory,
        ICofoundryDbConnectionManager cofoundryDbConnectionFactory
        )
    {
        _loggerFactory = loggerFactory;
        _cofoundryDbConnectionFactory = cofoundryDbConnectionFactory;
    }

    /// <summary>
    /// Used to configure the default settings for an EF DbContext that connects 
    /// to the Cofoundry database. This default implementation will set up the
    /// logger and use the shared Cofoundry connection so that any DbContext
    /// initialized this way can enlist in the same transaction.
    /// </summary>
    /// <param name="dbContext">
    /// The DbContext instance to configure. Since the DbContext will be uninitialized
    /// this is mainly provided in case the initializer needs to reflect on the
    /// Dbcontext type.
    /// </param>
    /// <param name="optionsBuilder">
    /// The options builder parameter to the OnConfiguring method where this 
    /// initializer is invoked.
    /// </param>
    /// <returns>
    /// DbContextOptionsBuilder instance for method chaining.
    /// </returns>
    public DbContextOptionsBuilder Configure(DbContext dbContext, DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(optionsBuilder);

        optionsBuilder.UseLoggerFactory(_loggerFactory);

        var connection = _cofoundryDbConnectionFactory.GetShared();
        optionsBuilder.UseSqlServer(connection);

        return optionsBuilder;
    }
}
