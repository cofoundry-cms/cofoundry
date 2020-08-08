using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Common mapping functionality for PageSummaries
    /// </summary>
    public interface IPageSummaryMapper
    {
        /// <summary>
        /// Finishes off bulk mapping of tags and page routes in a PageSummary object
        /// </summary>
        Task<List<PageSummary>> MapAsync(ICollection<Page> dbPages, IExecutionContext executionContext);
    }
}
