using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper.QueryableExtensions;
using System.IO;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns a web directory with the specified id as a WebDirectoryRoute instance.
    /// </summary>
    public class GetWebDirectoryRouteByIdQueryHandler 
        : IQueryHandler<GetByIdQuery<WebDirectoryRoute>, WebDirectoryRoute>
        , IAsyncQueryHandler<GetByIdQuery<WebDirectoryRoute>, WebDirectoryRoute>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<WebDirectoryRoute>, WebDirectoryRoute>
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;

        public GetWebDirectoryRouteByIdQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region execution

        public WebDirectoryRoute Execute(GetByIdQuery<WebDirectoryRoute> query, IExecutionContext executionContext)
        {
            var result = _queryExecutor
                .GetAll<WebDirectoryRoute>()
                .SingleOrDefault(l => l.WebDirectoryId == query.Id);

            return result;
        }

        public async Task<WebDirectoryRoute> ExecuteAsync(GetByIdQuery<WebDirectoryRoute> query, IExecutionContext executionContext)
        {
            var result = (await _queryExecutor
                .GetAllAsync<WebDirectoryRoute>())
                .SingleOrDefault(l => l.WebDirectoryId == query.Id);

            return result;
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<WebDirectoryRoute> command)
        {
            yield return new WebDirectoryReadPermission();
        }

        #endregion
    }
}
