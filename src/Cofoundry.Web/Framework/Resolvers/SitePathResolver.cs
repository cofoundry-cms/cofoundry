using Cofoundry.Core;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cofoundry.Web
{
    /// <summary>
    /// A path resolver that uses the asp.net hosting environment to get the application
    /// content root path and use it to resolve relative paths to absolute paths.
    /// </summary>
    public class SitePathResolver : IPathResolver
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public SitePathResolver(
            IHostingEnvironment hostingEnvironment
            )
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public string MapPath(string path)
        {
            var root = _hostingEnvironment.ContentRootPath;
            if (string.IsNullOrWhiteSpace(path)) return root;
            path = path.TrimStart('~').TrimStart(new char[] { '/', '\\' });

            var combinedPath = Path.Combine(root, path);

            return combinedPath;
        }
    }
}
