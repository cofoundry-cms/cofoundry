using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// A command that updates a part of a module or application to a
    /// specific version number.
    /// </summary>
    public interface IVersionedUpdateCommand
    {
        /// <summary>
        /// The version of the command to run. Version numbers
        /// should start at 1.
        /// </summary>
        int Version { get; }

        /// <summary>
        /// Short description of the command being run, used for log record identification purposes.
        /// </summary>
        string Description { get; }
    }
}
