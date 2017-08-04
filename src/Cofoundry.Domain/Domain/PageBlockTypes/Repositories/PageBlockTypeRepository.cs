using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple facade over page data access queries/commands to them more discoverable
    /// in implementations.
    /// </summary>
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
        
        public Task<IEnumerable<PageBlockTypeSummary>> GetAllPageBlockTypeSummariesAsync(IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetAllAsync<PageBlockTypeSummary>(executionContext);
        }

        public Task<PageBlockTypeSummary> GetPageBlockTypeSummaryByIdAsync(int pageBlockTypeId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdAsync<PageBlockTypeSummary>(pageBlockTypeId, executionContext);
        }

        public Task<PageBlockTypeDetails> GetPageBlockTypeDetailsByIdAsync(int pageBlockTypeId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdAsync<PageBlockTypeDetails>(pageBlockTypeId, executionContext);
        }

        #endregion
    }
}
