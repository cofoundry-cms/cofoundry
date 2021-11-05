using Cofoundry.Core.EntityFramework;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data.Internal
{
    /// <inheritdoc/>
    public class PageDirectoryStoredProcedures : IPageDirectoryStoredProcedures
    {
        private readonly IEntityFrameworkSqlExecutor _entityFrameworkSqlExecutor;
        private readonly CofoundryDbContext _dbContext;

        public PageDirectoryStoredProcedures(
            IEntityFrameworkSqlExecutor entityFrameworkSqlExecutor,
            CofoundryDbContext dbContext
            )
        {
            _entityFrameworkSqlExecutor = entityFrameworkSqlExecutor;
            _dbContext = dbContext;
        }

        public Task UpdatePageDirectoryClosureAsync()
        {
            return _entityFrameworkSqlExecutor
                .ExecuteCommandAsync(_dbContext,
                "Cofoundry.PageDirectoryClosure_Update"
                 );
        }

        public Task UpdatePageDirectoryPathAsync()
        {
            return _entityFrameworkSqlExecutor
                .ExecuteCommandAsync(_dbContext,
                "Cofoundry.PageDirectoryPath_Update"
                 );
        }
    }
}
