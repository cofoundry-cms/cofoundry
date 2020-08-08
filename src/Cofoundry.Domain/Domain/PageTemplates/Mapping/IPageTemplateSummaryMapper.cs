using Cofoundry.Domain.QueryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to PageTemplateSummary objects.
    /// </summary>
    public interface IPageTemplateSummaryMapper
    {
        /// <summary>
        /// Maps an EF PageTemplate record from the db into an PageTemplateSummary 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="queryModel">Query model with data from the database.</param>
        PageTemplateSummary Map(PageTemplateSummaryQueryModel queryModel);
    }
}
