using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to PageBlockTypeSummary objects.
    /// </summary>
    public interface IPageBlockTypeSummaryMapper
    {
        /// <summary>
        /// Maps an EF PageBlockType record from the db into an PageBlockTypeSummary 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbPageBlockType">PageBlockType record from the database.</param>
        PageBlockTypeSummary Map(PageBlockType dbPageBlockType);
    }
}
