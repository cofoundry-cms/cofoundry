using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Indicates that an entity model has some kind of numerical 
    /// ordering. Use EntityOrderableHelper to assist with ordering
    /// collections of IEntityOrderable instances.
    /// </summary>
    public interface IEntityOrderable
    {
        /// <summary>
        /// The numerical order of the entity in a collection.
        /// </summary>
        int Ordering { get; set; }
    }
}
