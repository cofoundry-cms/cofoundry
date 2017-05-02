using Cofoundry.Core.ResourceFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Helper to load view files from a path.
    /// </summary>
    public class ViewFileReader : IViewFileReader
    {
        #region constructor

        private readonly IResourceLocator _resourceLocator;

        public ViewFileReader(
            IResourceLocator resourceLocator
            )
        {
            _resourceLocator = resourceLocator;
        }

        #endregion

        /// <summary>
        /// Attempts to read a view file to a string, returning null if the file does not exist.
        /// </summary>
        /// <param name="path">The virtual path to the view file.</param>
        public async Task<string> ReadViewFileAsync(string path)
        {
            string result = null;

            if (!FileExists(path)) return result;

            var file = _resourceLocator.GetFile(path);
            if (file == null) return null;

            using (var stream = file.Open())
            using (var reader = new StreamReader(stream))
            {
                result = await reader.ReadToEndAsync();
            }

            return result;
        }

        private bool FileExists(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            // check well formatted path
            if (path[0] != '~' && path[0] != '/') return false;

            return _resourceLocator.FileExists(path);
        }
    }
}
