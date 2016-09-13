using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core
{
    public static class CofoundryModuleInfo
    {
        /// <summary>
        /// AutoUpdate module identifier for Cofoundry, make sure you 
        /// have this as a dependency in any UpdatePackages you create
        /// that have a dependency on Cofoundry installation.
        /// </summary>
        public const string ModuleIdentifier = "Cofoundry";
    }
}
