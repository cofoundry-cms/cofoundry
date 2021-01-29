using Cofoundry.Domain.QueryModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to PageGroupSummary objects.
    /// </summary>
    public interface IPageGroupSummaryMapper
    {
        /// <summary>
        /// Maps an EF PageGroup record from the db into an PageGroupSummary 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="queryModel">Query model with data from the database.</param>
        PageGroupSummary Map(PageGroupSummaryQueryModel queryModel);
    }
}
