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
        IEnumerable<PageModuleTypeSummary> GetAllPageModuleTypeSummaries(IExecutionContext executionContext = null);
        Task<IEnumerable<PageModuleTypeSummary>> GetAllPageModuleTypeSummariesAsync(IExecutionContext executionContext = null);
        PageModuleTypeSummary GetPageModuleTypeSummaryById(int id, IExecutionContext executionContext = null);
    }
}
