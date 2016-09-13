using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.EmbeddedResources
{
    /// <summary>
    /// Represents a file directory, which would be a collection of embedded resources
    /// in an assembly or a physical directory on disk
    /// </summary>
    public interface IResourceDirectory
    {
        /// <summary>
        /// Returns a collection of files in the directory.
        /// </summary>
        IEnumerable<IResourceFile> GetFiles();

        /// <summary>
        /// Returns a collection of child directories within the directory.
        /// </summary>
        IEnumerable<IResourceDirectory> GetDirectories();

        string VirtualPath { get; }
    }
}
