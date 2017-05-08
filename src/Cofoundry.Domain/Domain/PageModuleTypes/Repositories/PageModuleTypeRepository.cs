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
    public class PageModuleTypeRepository : IPageModuleTypeRepository
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;

        public PageModuleTypeRepository(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region queries
        
        public Task<IEnumerable<PageModuleTypeSummary>> GetAllPageModuleTypeSummariesAsync(IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetAllAsync<PageModuleTypeSummary>(executionContext);
        }

        public Task<PageModuleTypeSummary> GetPageModuleTypeSummaryByIdAsync(int id, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdAsync<PageModuleTypeSummary>(id, executionContext);
        }

        public Task<PageModuleTypeDetails> GetPageModuleTypeDetailsByIdAsync(int id, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdAsync<PageModuleTypeDetails>(id, executionContext);
        }

        #endregion
    }
}
