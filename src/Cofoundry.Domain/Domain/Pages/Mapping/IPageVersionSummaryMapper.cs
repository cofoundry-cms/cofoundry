using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple mapper for mapping to PageVersionSummary objects.
    /// </summary>
    public interface IPageVersionSummaryMapper
    {
        /// <summary>
        /// Maps an EF PageVersion record from the db into an PageVersionSummary 
        /// object.
        /// </summary>
        /// <param name="dbPageVersion">PageVersion record from the database.</param>
        PageVersionSummary Map(PageVersion dbPageVersion);
    }
}
