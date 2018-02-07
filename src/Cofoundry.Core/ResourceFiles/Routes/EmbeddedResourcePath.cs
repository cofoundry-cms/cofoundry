using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Cofoundry.Core.ResourceFiles
{
    /// <summary>
    /// Represents a path to a set of embedded resources E.g. '/parent/child/content'
    /// in a specific assembly.
    /// </summary>
    public struct EmbeddedResourcePath
    {
        public EmbeddedResourcePath(
            Assembly assembly,
            string path
            )
        {
            Assembly = assembly;
            Path = path;
        }

        /// <summary>
        /// The assembly containing the resources.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// The path containing embedded resources. E.g. '/parent/child/content'
        /// </summary>
        public string Path { get; }
    }
}
