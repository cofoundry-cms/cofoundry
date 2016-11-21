using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Helper to load view files from a path.
    /// </summary>
    public interface IViewFileReader
    {
        /// <summary>
        /// Attempts to read a view file to a string, returning null if the file does not exist.
        /// </summary>
        /// <param name="path">The virtual path to the view file.</param>
        Task<string> ReadViewFileAsync(string path);
    }
}
