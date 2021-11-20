using Cofoundry.Core;
using Cofoundry.Core.AutoUpdate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Samples.SPASite.Domain.Install
{
    /// <summary>
    /// Cofoundry has an automatic update system that runs when the app is 
    /// started. This mainly runs sql scripts, but it can also run .net 
    /// code too. Your app as well as any plugin can tap into this process 
    /// to run updates. This example uses a base class to use the default
    /// process for updating a database using sql scripts.
    /// 
    /// See https://www.cofoundry.org/docs/framework/auto-update
    /// </summary>
    public class UpdatePackageFactory : BaseDbOnlyUpdatePackageFactory
    {
        /// <summary>
        /// The module identifier should be unique to this installation
        /// and usually indicates the application or plugin being updated
        /// </summary>
        public override string ModuleIdentifier
        {
            get { return "SPASite"; }
        }

        /// <summary>
        /// Here we can any modules that this installation is dependent
        /// on. In this case we are dependent on the Cofoundry installation
        /// being run before this one
        /// </summary>
        public override ICollection<string> DependentModules { get; } = new string[] { CofoundryModuleInfo.ModuleIdentifier };
    }
}
