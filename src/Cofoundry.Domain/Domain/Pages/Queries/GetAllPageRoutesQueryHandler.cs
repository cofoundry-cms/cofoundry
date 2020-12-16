using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Returns a collection of page routing data for all pages. The 
    /// PageRoute projection is a small page object focused on providing 
    /// routing data only. Data returned from this query is cached by 
    /// default as it's core to routing and often incorporated in more detailed
    /// page projections.
    /// </summary>
    public class GetAllPageRoutesQueryHandler 
        : IQueryHandler<GetAllPageRoutesQuery, ICollection<PageRoute>>
        , IPermissionRestrictedQueryHandler<GetAllPageRoutesQuery, ICollection<PageRoute>>
    {
        private readonly IQueryExecutor _queryExecutor;

        public GetAllPageRoutesQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }
        
        public async Task<ICollection<PageRoute>> ExecuteAsync(GetAllPageRoutesQuery query, IExecutionContext executionContext)
        {
            var result = await _queryExecutor.ExecuteAsync(new GetPageRouteLookupQuery());
            return result.Values;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetAllPageRoutesQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
