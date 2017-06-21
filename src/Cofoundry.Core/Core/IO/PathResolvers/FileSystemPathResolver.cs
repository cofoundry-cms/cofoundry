using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// IPathResolver for resolving virtual paths relative to the executing assembly.
    /// This is mainly intended for resolving paths in console apps.
    /// </summary>
    public class FileSystemPathResolver : IPathResolver
    {
        public string MapPath(string path)
        {
            var basePath = AppContext.BaseDirectory;

            if (string.IsNullOrWhiteSpace(path)) return basePath;

            path = path.TrimStart(new char[] { '~', '/' });

            return Path.Combine(basePath, path);
        }
    }
}
