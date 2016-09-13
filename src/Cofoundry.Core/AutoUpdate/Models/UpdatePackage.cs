using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// Represents a set of updates for a particular module.
    /// </summary>
    public class UpdatePackage : IComparable<UpdatePackage>
    {
        #region constructor

        public UpdatePackage()
        {
            Commands = Enumerable.Empty<IUpdateCommand>();
            DependentModules = Enumerable.Empty<string>();
        }

        #endregion

        #region public properties

        /// <summary>
        /// Unique identifier for the module
        /// </summary>
        public string ModuleIdentifier { get; set; }

        /// <summary>
        /// Commands to run when updating the module
        /// </summary>
        public IEnumerable<IUpdateCommand> Commands { get; set; }

        /// <summary>
        /// A collection of module identifiers that this module is dependent on.
        /// </summary>
        public IEnumerable<string> DependentModules { get; set; }

        #endregion

        #region IComparable implementation

        /// <summary>
        /// Sorts packages so that dependent packages are first.
        /// </summary>
        public int CompareTo(UpdatePackage other)
        {
            const int BEFORE = -1;
            const int SAME = 0;
            const int AFTER = 1;

            if (!this.DependentModules.Any() && !other.DependentModules.Any()) return SAME;

            if (this.DependentModules.Contains(other.ModuleIdentifier)) return AFTER;
            if (other.DependentModules.Contains(this.ModuleIdentifier)) return BEFORE;

            return SAME;
        }

        #endregion
    }
}
