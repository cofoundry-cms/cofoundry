using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// Resolver for mapping application relative paths to absolute 
    /// physical paths.
    /// </summary>
    public interface IPathResolver
    {
        /// <summary>
        /// Returns the physical file path that corresponds to the specified relative 
        /// path. If the path value is null or empty then the application root path is 
        /// returned.
        /// </summary>
        /// <param name="path">virtual path to resolve</param>
        /// <returns></returns>
        string MapPath(string path);
    }
}
