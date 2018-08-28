using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Implement this interface on your custom entity definition class to 
    /// define a default sort type and sort direction. This apply to all
    /// queries where no sort type is specified and so is most useful for
    /// customizing the default ordering or items in the list view in the
    /// admin panel.
    /// </summary>
    public interface ISortedCustomEntityDefinition : ICustomEntityDefinition
    {
        /// <summary>
        /// The sorting to apply by default when querying collections of custom 
        /// entities of this type. A query can specify a sort type to override 
        /// this value.
        /// </summary>
        CustomEntityQuerySortType DefaultSortType { get; }

        /// <summary>
        /// The default sort direction to use when ordering with the
        /// default sort type.
        /// </summary>
        SortDirection DefaultSortDirection { get; }
    }
}
