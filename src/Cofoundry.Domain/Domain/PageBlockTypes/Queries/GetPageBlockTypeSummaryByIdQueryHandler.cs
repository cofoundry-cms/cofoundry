using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    public class GetPageBlockTypeSummaryByIdQueryHandler
        : IQueryHandler<GetPageBlockTypeSummaryByIdQuery, PageBlockTypeSummary>
        , IPermissionRestrictedQueryHandler<GetPageBlockTypeSummaryByIdQuery, PageBlockTypeSummary>
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;

        public GetPageBlockTypeSummaryByIdQueryHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        #endregion

        public async Task<PageBlockTypeSummary> ExecuteAsync(GetPageBlockTypeSummaryByIdQuery query, IExecutionContext executionContext)
        {
            var allBlockTypes = await _queryExecutor.ExecuteAsync(new GetAllPageBlockTypeSummariesQuery(), executionContext);
            return allBlockTypes.SingleOrDefault(t => t.PageBlockTypeId == query.PageBlockTypeId);
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageBlockTypeSummaryByIdQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
