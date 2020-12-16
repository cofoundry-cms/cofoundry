using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to PageBlockTypeDetails objects.
    /// </summary>
    public interface IPageBlockTypeDetailsMapper
    {
        /// <summary>
        /// Maps an EF PageBlockType record from the db into an PageBlockTypeDetails 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="blockTypeSummary">PageBlockType record from the database.</param>
        PageBlockTypeDetails Map(PageBlockTypeSummary blockTypeSummary);
    }
}
