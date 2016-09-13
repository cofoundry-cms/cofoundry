using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// Resolver for mapping app relative virtual paths to physical paths.
    /// </summary>
    public interface IPathResolver
    {
        /// <summary>
        /// Returns the physical file path that corresponds to the specified virtual path.
        /// </summary>
        /// <param name="path">virtual path to resolve</param>
        /// <returns></returns>
        string MapPath(string path);
    }
}
