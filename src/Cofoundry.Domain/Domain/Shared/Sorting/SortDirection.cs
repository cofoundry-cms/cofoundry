using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents the direction that sorting can be applied in a query.
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// Sort by the normal or expected arrangement. Often this is low to high
        /// sequence but in some cases such as publish or create date ordering then
        /// expected ordering is latest first.
        /// </summary>
        Default,

        /// <summary>
        /// Reverses the default sort direction.
        /// </summary>
        Reversed
    }
}
