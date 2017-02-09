using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents a permitted action on a specific entity e.g.
    /// "read pages" or "delete images"
    /// </summary>
    public interface IEntityPermission : IPermission
    {
        /// <summary>
        /// The definition object of the entity this permission represents
        /// </summary>
        IEntityDefinition EntityDefinition { get; }
    }
}
