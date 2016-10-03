using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// The basic component of an IPermission, the type represents
    /// the type of action to be permitted. Some common permission types
    /// like 'Read', 'Update' etc can be re-used when applied to an IEntityPermission
    /// </summary>
    public class PermissionType
    {
        /// <summary>
        /// Creates a new PermissionType instance
        /// </summary>
        public PermissionType()
        {
        }

        /// <summary>
        /// Creates a new PermissionType instance with the specified settings
        /// </summary>
        /// <param name="code">The unique 6 charachter key of the permission type</param>
        /// <param name="name">A user friendly name for the permission</param>
        /// <param name="description">A description to display against the permission in the user interface</param>
        public PermissionType(string code, string name, string description)
        {
            Code = code;
            Name = name;
            Description = description;
        }

        /// <summary>
        /// The unique 6 charachter key of the permission type
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// A user friendly name for the permission
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A description to display against the permission in the user interface
        /// </summary>
        public string Description { get; set; }
    }
}
