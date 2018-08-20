using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Cofoundry.Core.EntityFramework
{
    /// <summary>
    /// Used to configure the default settings for an EF DbContext that connects 
    /// to the Cofoundry database. The default implementation will set up the
    /// logger and use the shared Cofoundry connection so that any DbContext
    /// initialized this way can enlist in the same transaction.
    /// </summary>
    public interface ICofoundryDbContextInitializer
    {
        /// <summary>
        /// Used to configure the default settings for an EF DbContext that connects 
        /// to the Cofoundry database. The default implementation will set up the
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
        DbContextOptionsBuilder Configure(DbContext dbContext, DbContextOptionsBuilder optionsBuilder);
    }
}
