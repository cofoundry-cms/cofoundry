using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to PageTemplateMicroSummary objects.
    /// </summary>
    public interface IPageTemplateMicroSummaryMapper
    {
        /// <summary>
        /// Maps an EF PageTemplate record from the db into an PageTemplateMicroSummary 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbPageTemplate">PageTemplate record from the database.</param>
        PageTemplateMicroSummary Map(PageTemplate dbPageTemplate);
    }
}
