using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Formatter used to append cache-breaking version parameters
    /// to a static resource uri. This is basically an injectable version
    /// of the "asp-append-version" tag helper attribute that works across 
    /// all registered static file providers. Note that the build in tag
    /// helper only works for IHostingEnvironment.WebRootFileProvider
    /// </summary>
    public class StaticFilePathFormatter : IStaticFilePathFormatter
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IStaticResourceFileProvider _staticResourceFileProvider;

        public StaticFilePathFormatter(
            IMemoryCache memoryCache,
            IStaticResourceFileProvider staticResourceFileProvider
            )
        {
            _memoryCache = memoryCache;
            _staticResourceFileProvider = staticResourceFileProvider;
        }

        /// <summary>
        /// Appends a version hash querystring parameter to the end
        /// of the file path, e.g. 'myfile.js?v=examplecomputedhash'
        /// </summary>
        /// <param name="applicationRelativePath">
        /// The static resource file path, which must be the full application 
        /// relative path.
        /// </param>
        /// <returns>
        /// If the file is found, the path is returned with the version 
        /// appended, otherwise the unmodified path is returned.
        /// </returns>
        public string AppendVersion(string applicationRelativePath)
        {
            // Only support site relative urls, so no need to pass requestPathBase;
            var versionProvider = new CofoundryFileVersionProvider(_staticResourceFileProvider, _memoryCache);
            var path = versionProvider.AddFileVersionToPath(null, applicationRelativePath);

            return path;
        }
    }
}