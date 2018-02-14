using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// A command that creates one or more directories in the file system.
    /// </summary>
    public class CreateDirectoriesUpdateCommand : IVersionedUpdateCommand
    {
        /// <summary>
        /// The version of the command to run. Version numbers
        /// should start at 1.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// A collection of directory paths to create. Resolved using IPathResolver.
        /// </summary>
        public ICollection<string> Directories { get; set; }

        /// <summary>
        /// Short description of the command being run, used for log record identification purposes.
        /// </summary>
        public string Description { get; set; }
    }
}
