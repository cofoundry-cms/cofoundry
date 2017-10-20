using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageBlockTypeSummaryByIdHandler
        : IAsyncQueryHandler<GetByIdQuery<PageBlockTypeSummary>, PageBlockTypeSummary>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<PageBlockTypeSummary>, PageBlockTypeSummary>
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;

        public GetPageBlockTypeSummaryByIdHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        #endregion

        public async Task<PageBlockTypeSummary> ExecuteAsync(GetByIdQuery<PageBlockTypeSummary> query, IExecutionContext executionContext)
        {
            var allBlockTypes = await _queryExecutor.GetAllAsync<PageBlockTypeSummary>();
            return allBlockTypes.SingleOrDefault(t => t.PageBlockTypeId == query.Id);
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<PageBlockTypeSummary> query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
