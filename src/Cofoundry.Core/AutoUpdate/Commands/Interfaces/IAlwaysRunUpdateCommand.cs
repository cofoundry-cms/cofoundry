using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// A command that always runs after the AutoUpdater has run
    /// </summary>
    public interface IAlwaysRunUpdateCommand
    {
        /// <summary>
        /// Short description of the command being run, used for log record 
        /// identification purposes. Max 200 characters.
        /// </summary>
        string Description { get; }
    }
}
