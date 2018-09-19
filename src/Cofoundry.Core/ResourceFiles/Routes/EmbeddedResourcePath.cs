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
    public class EmbeddedResourcePath
    {
        public EmbeddedResourcePath(
            Assembly assembly,
            string path,
            string rewriteFrom = null
            )
        {
            Assembly = assembly;
            Path = path;
            RewriteFrom = rewriteFrom;
        }

        /// <summary>
        /// The assembly containing the resources.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// The path containing embedded resources. E.g. '/parent/child/content'
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Optional inbound url route to rewrite the path from e.g. /virtual/path/to/content
        /// </summary>
        public string RewriteFrom { get; set; }
    }
}
