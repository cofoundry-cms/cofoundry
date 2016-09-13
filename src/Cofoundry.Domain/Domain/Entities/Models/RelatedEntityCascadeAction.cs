using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public enum RelatedEntityCascadeAction
    {
        /// <summary>
        /// Take no action, the root entity will be prevented from being deleted.
        /// </summary>
        None = 1,
        /// <summary>
        /// Warns the user that deleting the entity will cause a cacading deletion
        /// giving them the option to cancel. If the deletion is made, the related entity
        /// will be removed from the root entity. If the property is required then selecting this
        /// option will throw an exception.
        /// </summary>
        CascadeProperty = 2
    }
}
