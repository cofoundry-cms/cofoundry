using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// Represents a version marker for a Module.
    /// </summary>
    public class ModuleVersion
    {
        /// <summary>
        /// Unique Identifer for the module
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// The version number this class represents.
        /// </summary>
        public int Version { get; set; }
    }
}
