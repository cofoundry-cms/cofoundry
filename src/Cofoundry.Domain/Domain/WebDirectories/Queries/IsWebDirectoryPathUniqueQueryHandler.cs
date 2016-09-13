using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class IsWebDirectoryPathUniqueQueryHandler 
        : IQueryHandler<IsWebDirectoryPathUniqueQuery, bool>
        , IAsyncQueryHandler<IsWebDirectoryPathUniqueQuery, bool>
        , IPermissionRestrictedQueryHandler<IsWebDirectoryPathUniqueQuery, bool>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public IsWebDirectoryPathUniqueQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution

        public bool Execute(IsWebDirectoryPathUniqueQuery query, IExecutionContext executionContext)
        {
            var exists = Query(query).Any();
            return !exists;
        }

        public async Task<bool> ExecuteAsync(IsWebDirectoryPathUniqueQuery query, IExecutionContext executionContext)
        {
            var exists = await Query(query).AnyAsync();
            return !exists;
        }

        #endregion

        #region helpers

        private IQueryable<WebDirectory> Query(IsWebDirectoryPathUniqueQuery query)
        {
            return _dbContext
                .WebDirectories
                .AsNoTracking()
                .Where(d => d.WebDirectoryId != query.WebDirectoryId
                    && d.IsActive
                    && d.UrlPath == query.UrlPath
                    && d.ParentWebDirectoryId == query.ParentWebDirectoryId
                    );
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(IsWebDirectoryPathUniqueQuery command)
        {
            yield return new WebDirectoryReadPermission();
        }

        #endregion
    }

}
