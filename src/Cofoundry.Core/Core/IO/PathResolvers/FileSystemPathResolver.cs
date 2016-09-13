using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// IPathResolver for resolving virtual paths relative to the executing assembly.
    /// This is mainly intended for resolving paths in console apps.
    /// </summary>
    public class FileSystemPathResolver : IPathResolver
    {
        private readonly string basePath = Path.GetDirectoryName(new Uri((System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).LocalPath);

        public string MapPath(string path)
        {
            if (path.StartsWith("~/"))
            {
                path = path.Substring(2);
            }

            return System.IO.Path.Combine(basePath, path);
        }
    }
}
