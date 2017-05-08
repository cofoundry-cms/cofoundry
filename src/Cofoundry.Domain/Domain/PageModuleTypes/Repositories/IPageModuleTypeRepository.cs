using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple facade over page data access queries/commands to them more discoverable
    /// in implementations.
    /// </summary>
    public interface IPageModuleTypeRepository
    {
        Task<IEnumerable<PageModuleTypeSummary>> GetAllPageModuleTypeSummariesAsync(IExecutionContext executionContext = null);
        Task<PageModuleTypeSummary> GetPageModuleTypeSummaryByIdAsync(int id, IExecutionContext executionContext = null);
    }
}
