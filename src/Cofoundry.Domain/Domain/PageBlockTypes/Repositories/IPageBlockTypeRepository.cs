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
    public interface IPageBlockTypeRepository
    {
        Task<IEnumerable<PageBlockTypeSummary>> GetAllPageBlockTypeSummariesAsync(IExecutionContext executionContext = null);

        Task<PageBlockTypeSummary> GetPageBlockTypeSummaryByIdAsync(int pageBlockTypeId, IExecutionContext executionContext = null);

        Task<PageBlockTypeDetails> GetPageBlockTypeDetailsByIdAsync(int pageBlockTypeId, IExecutionContext executionContext = null);
    }
}
