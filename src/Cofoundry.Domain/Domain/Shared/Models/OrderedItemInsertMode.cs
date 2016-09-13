using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used to decide where to replace an item in a collection
    /// of ordered items.
    /// </summary>
    public enum OrderedItemInsertMode
    {
        /// <summary>
        /// Insert at the end of the collection
        /// </summary>
        Last,

        /// <summary>
        /// Insert at the start of the collection
        /// </summary>
        First,

        /// <summary>
        /// Insert before a nominated item in the collection
        /// </summary>
        BeforeItem,

        /// <summary>
        /// Insert after a nominated item in the collection
        /// </summary>
        AfterItem
    }
}
