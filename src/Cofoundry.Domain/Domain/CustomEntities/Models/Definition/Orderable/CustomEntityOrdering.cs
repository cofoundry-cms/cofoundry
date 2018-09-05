using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// The type of ordering allowed on a custom entity
    /// </summary>
    public enum CustomEntityOrdering
    {
        /// <summary>
        /// No custom ordering
        /// </summary>
        None,

        /// <summary>
        /// Partial ordering where an ordering is specified for a subset of 
        /// entities and the rest take a natural ordering. Implement ISortedCustomEntityDefinition
        /// to define the seondary level ordering of items that have not been ordered.
        /// </summary>
        /// <remarks>
        /// Partial ordering is actually the same as full ordering as far as the db is concerned,
        /// the only difference is how the ordering is managed on the front-end whereby partial ordering
        /// is opt-in ordering and full ordering includes all entities (up to 60)
        /// </remarks>
        Partial,

        /// <summary>
        /// Each custom entity will have an ordering set. This is only intended for smaller
        /// lists of entities and will not work for collections with more than 60 elements.
        /// </summary>
        Full
    }
}
