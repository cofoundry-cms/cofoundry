using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Determines if a page directory UrlPath is unique
    /// within its parent directory.
    /// </summary>
    public class IsPageDirectoryPathUniqueQueryHandler
        : IQueryHandler<IsPageDirectoryPathUniqueQuery, bool>
        , IPermissionRestrictedQueryHandler<IsPageDirectoryPathUniqueQuery, bool>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public IsPageDirectoryPathUniqueQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution
        
        public async Task<bool> ExecuteAsync(IsPageDirectoryPathUniqueQuery query, IExecutionContext executionContext)
        {
            var exists = await Query(query).AnyAsync();
            return !exists;
        }

        #endregion

        #region helpers

        private IQueryable<PageDirectory> Query(IsPageDirectoryPathUniqueQuery query)
        {
            return _dbContext
                .PageDirectories
                .AsNoTracking()
                .Where(d => d.PageDirectoryId != query.PageDirectoryId
                    && d.UrlPath == query.UrlPath
                    && d.ParentPageDirectoryId == query.ParentPageDirectoryId
                    );
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(IsPageDirectoryPathUniqueQuery command)
        {
            yield return new PageDirectoryReadPermission();
        }

        #endregion
    }

}
