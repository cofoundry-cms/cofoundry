using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple facade over page data access queries/commands to them more discoverable
    /// in implementations.
    /// </summary>
    [Obsolete("Use the new IContentRepository instead.")]
    public class PageBlockTypeRepository : IPageBlockTypeRepository
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;

        public PageBlockTypeRepository(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region queries
        
        public Task<ICollection<PageBlockTypeSummary>> GetAllPageBlockTypeSummariesAsync(IExecutionContext executionContext = null)
        {
            var query = new GetAllPageBlockTypeSummariesQuery();
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public Task<PageBlockTypeSummary> GetPageBlockTypeSummaryByIdAsync(int pageBlockTypeId, IExecutionContext executionContext = null)
        {
            var query = new GetPageBlockTypeSummaryByIdQuery(pageBlockTypeId);
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        public Task<PageBlockTypeDetails> GetPageBlockTypeDetailsByIdAsync(int pageBlockTypeId, IExecutionContext executionContext = null)
        {
            var query = new GetPageBlockTypeDetailsByIdQuery(pageBlockTypeId);
            return _queryExecutor.ExecuteAsync(query, executionContext);
        }

        #endregion
    }
}
