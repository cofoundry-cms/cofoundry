using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Cofoundry.Web.ModularMvc
{
    /// <summary>
    /// Represents the location of a recource embedded in an assembly.
    /// </summary>
    public class AssemblyVirtualFileLocation
    {
        /// <summary>
        /// The assembly the resource is embedded in
        /// </summary>
        public Assembly Assembly { get; set; }

        /// <summary>
        /// The full path to the resource
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The path to the resource with the assembly name removed
        /// from the start. This value needs to be unique across all
        /// assemblies.
        /// </summary>
        public string PathWithoutAssemblyName { get; set; }

        /// <summary>
        /// The resource named transformed to a virtual path, i.e. "~/mydirectory/myfile.js"
        /// </summary>
        public string VirtualPath { get; set; }
    }
}