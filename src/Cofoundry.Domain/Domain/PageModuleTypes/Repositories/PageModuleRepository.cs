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
    public class PageModuleRepository : IPageModuleRepository
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;

        public PageModuleRepository(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region queries

        public IEnumerable<PageModuleTypeSummary> GetAllPageModuleTypeSummaries(IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetAll<PageModuleTypeSummary>(executionContext);
        }

        public Task<IEnumerable<PageModuleTypeSummary>> GetAllPageModuleTypeSummariesAsync(IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetAllAsync<PageModuleTypeSummary>(executionContext);
        }

        public PageModuleTypeSummary GetPageModuleTypeSummaryById(int id, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetById<PageModuleTypeSummary>(id, executionContext);
        }

        public Task<PageModuleTypeDetails> GetPageModuleTypeDetailsByIdAsync(int id, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdAsync<PageModuleTypeDetails>(id, executionContext);
        }

        #endregion
    }
}
